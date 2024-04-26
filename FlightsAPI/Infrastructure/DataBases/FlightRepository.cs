using AutoMapper;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.FlightDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.Infrastructure.DataBases
{
	public class FlightRepository(
		FlightDbContext DbContext,
		IOptions<FlightDbOptions> IOptions,
		IOptions<JsonSerializerOptions> JOptions,
		IMapper Mapper) : IFlightRepository
	{
		private const string DEFAULT_TICKET_NO = "IS_SET_IN_DB"; //Don't change. Same value is hardcoded in database.
		private readonly FlightDbOptions _options = IOptions.Value;
		private readonly JsonSerializerOptions _jOptions = JOptions.Value;
		public async Task<BookingResult> BookFlights(BookingOrder query)
		{
			try
			{
				TicketFlight[] ticketFlights = CreateTicketFlights(query);
				Ticket ticket = CreateTicket(query, ticketFlights);
				Booking booking = CreateBooking(query, ticket);

				DbContext.Bookings.Add(booking);
				await DbContext.SaveChangesAsync();

				return new BookingResult
				{
					Id = GenerateBookingId(booking),
					AssociatedRecords = [new AssosiatedRecord {
						CreationDate = booking.BookDate.ToString("yyyy-MM-ddTHH:mm:ss"),
						FlightOfferId = query.FlightOffer?.Id,
						Reference = booking.BookRef
					}]
				};
			}
			catch (DbUpdateException ex)
			{
				if (ex.InnerException is Npgsql.PostgresException npgEx)
				{
					return new BookingResult { Issues = [new OrderIssue { Title = "PostgreSQLException", Detail = npgEx.Message }] };
				}
				throw;
			}
		}

		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			var parameters = new Parameters(query, _options);

			Flight[][] outRoutes = await RetrieveOutboundRoutesAsync(query, parameters);

			if (query.ReturnDate == null)
				return MapToFlightOffers(outRoutes);

			var routeCombinations = await RetrieveRouteCombinationsAsync(outRoutes, query, parameters);

			return MapToFlightOffers(routeCombinations); ;
		}


		#region Private
		#region BookFlights
		private static TicketFlight[] CreateTicketFlights(BookingOrder query) =>
			query.FlightOffer?.TravelerPricings?.FirstOrDefault()?.FareDetailsBySegment?
				.Select(fd => new TicketFlight
				{
					FlightId = int.TryParse(fd.SegmentId, out int id) ? id : 0,
					Amount = fd.Price ?? -1,
					FareConditions = fd.Cabin ?? "",
					TicketNo = DEFAULT_TICKET_NO
				})
				.ToArray() ?? [];
			
		private Booking CreateBooking(BookingOrder query, Ticket ticket) =>
			new Booking
			{
				BookRef = Booking.GetRandomBookRef(),
				BookDate = DateTime.UtcNow,
				TotalAmount = (query.FlightOffer?.Price?.Total ?? 0) * _options.UsdToRubConversionRate,
				Tickets = [ticket]
			};
		private static string GenerateBookingId(Booking booking)
		{
			string originalString = $"{booking.BookRef}|{FlightProvider.DemoDB}|{booking.BookDate:yyyy-MM-dd}";
			byte[] bytes = Encoding.UTF8.GetBytes(originalString);

			return Convert.ToBase64String(bytes);
		}
		private Ticket CreateTicket(BookingOrder query, TicketFlight[] ticketFlights)
		{
			var contactData = new
			{
				Email = query.Traveler?.Contact?.EmailAddress ?? "",
				Phone = query.Traveler?.Contact?.Phone?.PhoneString ?? ""
			};

			var ticket = new Ticket
			{
				TicketNo = DEFAULT_TICKET_NO,
				PassengerName = query.Traveler?.Name?.FullName?.ToUpper() ?? "",
				ContactData = JsonSerializer.Serialize(contactData, _jOptions),
				PassengerId = query.Traveler?.PassengerId ?? "",
				TicketFlights = ticketFlights
			};

			return ticket;
		}
		#endregion


		#region GetFlightOffers
		private async Task<(Flight[], Flight[])[]> RetrieveRouteCombinationsAsync(Flight[][] outRoutes, FlightQuery query, Parameters parameters)
		{
			var tasks = outRoutes.Select(async outRoute =>
			{
				var returnRoutes = await RetrieveReturnRoutesAsync(outRoute, query, parameters);
				return AssembleCombinations(outRoute, returnRoutes);
			});

			return await FlattenCombinationsAsync(tasks, parameters);
		}

		private static async Task<(Flight[], Flight[])[]> FlattenCombinationsAsync(IEnumerable<Task<(Flight[], Flight[])[]>> tasks, Parameters parameters)
		{
			//Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance.
			//https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues

			List<(Flight[], Flight[])> combinations = [];

			foreach (var task in tasks)
			{
				var combs = await task;

				int remainingSlots = parameters.MaxFlightOffers - combinations.Count;
				int toTake = Math.Min(remainingSlots, combs.Length);

				combinations.AddRange(combs.Take(toTake));

				if (combinations.Count >= parameters.MaxFlightOffers)
					break;
			}
			return combinations.ToArray();
		}

		/// <summary>
		/// Produce an array of combinations related to the given outbound route. A combination is a tuple of arrays of flights, each array representing a one-way route
		/// </summary>
		private static (Flight[], Flight[])[] AssembleCombinations(Flight[] outRoute, Flight[][] returnRoutes) =>
			returnRoutes.Select(retRoute => (outRoute, retRoute)).ToArray();

		/// <summary>
		/// Retrieve the outbound routes. Each route is an array of sequential flights (segments). Only 1 segment routes (with no connections) are now supported.
		/// </summary>
		private async Task<Flight[][]> RetrieveOutboundRoutesAsync(FlightQuery query, Parameters parameters) =>
			await DbContext.Flights
				.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= parameters.MaxPriceRub))
				.Where(CombineExpressions(
					GetBasicOutFliter(query, parameters),
					GetAirlinesFilter(query)))
				.Take(parameters.MaxFlightOffers)
				.Select(fl => new Flight[] { fl })
				.ToArrayAsync();

		/// <summary>
		/// Retrieve return routes for the given outbound route. Each route is an array of sequential flights (segments). Only 1 segment routes (with no connections) are now supported.
		/// </summary>
		private async Task<Flight[][]> RetrieveReturnRoutesAsync(Flight[] outRoute, FlightQuery query, Parameters parameters) =>
			await DbContext.Flights
				.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= parameters.MaxPriceRub))
				.Where(CombineExpressions(
					GetBasicReturnFilter(query, outRoute, parameters),
					GetAirlinesFilter(query)))
				.Take(_options.MaxReturnFlights)
				.Select(fl => new Flight[] { fl })
				.ToArrayAsync();

		private IEnumerable<FlightOffer> MapToFlightOffers(Flight[][] outRoutes) =>
			Mapper.Map<IEnumerable<FlightOffer>>(outRoutes)
			.SetFlightOfferIds();
		private IEnumerable<FlightOffer> MapToFlightOffers((Flight[], Flight[])[] routeCombinations) =>
			Mapper.Map<IEnumerable<FlightOffer>>(routeCombinations)
			.SetFlightOfferIds();


		private Expression<Func<Flight, bool>> GetBasicOutFliter(FlightQuery query, Parameters parameters) =>
			fl =>
				fl.DepartureAirport == query.OriginLocationCode &&
				fl.ArrivalAirport == query.DestinationLocationCode &&
				fl.ScheduledDeparture.UtcDateTime.Date >= parameters.MinOutDate &&
				fl.ScheduledDeparture.UtcDateTime.Date <= parameters.MaxOutDate &&
				fl.TicketFlights.Any(); //skip flights with tickets unavailable;
		private Expression<Func<Flight, bool>> GetBasicReturnFilter(FlightQuery query, Flight[] outRoute, Parameters parameters)
		{
			var minReturnDepartTime = outRoute[^1].ScheduledArrival.UtcDateTime.AddMinutes(_options.MCT); //minimal acceptable time between two consequent flights
			return fl =>
				fl.DepartureAirport == query.DestinationLocationCode &&
				fl.ArrivalAirport == query.OriginLocationCode &&
				fl.ScheduledDeparture.UtcDateTime.Date >= parameters.MinReturnDate &&
				fl.ScheduledDeparture.UtcDateTime >= minReturnDepartTime &&
				fl.ScheduledDeparture.UtcDateTime.Date <= parameters.MaxReturnDate &&
				fl.TicketFlights.Any(); //skip flights with tickets unavailable;
		}
		private Expression<Func<Flight, bool>> GetAirlinesFilter(FlightQuery query)
		{
			var sc = query.SearchCriteria;
			return fl =>
				sc == null || (
					(
						sc.IncludedCarrierCodes == null ||
						sc.IncludedCarrierCodes.Length == 0 ||
						sc.IncludedCarrierCodes.Contains(fl.FlightNo.Substring(0, 2))
					)
					&&
					(
						sc.ExcludedCarrierCodes == null ||
						sc.ExcludedCarrierCodes.Length == 0 ||
						!sc.ExcludedCarrierCodes.Contains(fl.FlightNo.Substring(0, 2))
					)
				);
		}

		private static Expression<T> CombineExpressions<T>(Expression<T> firstExpression, Expression<T> secondExpression)
		{
			if (firstExpression is null)
				return secondExpression;

			if (secondExpression is null)
				return firstExpression;

			var invokedExpression = Expression.Invoke(secondExpression, firstExpression.Parameters);
			var combinedExpression = Expression.AndAlso(firstExpression.Body, invokedExpression);

			return Expression.Lambda<T>(combinedExpression, firstExpression.Parameters);
		}

		private record Parameters
		{
			/// <summary>
			/// Maximal acceptable price for flightOffer
			/// </summary>
			public decimal MaxPriceRub { get; init; }
			/// <summary>
			/// Maximal number of flight offers per flights provider
			/// </summary>
			public int MaxFlightOffers { get; init; }
			/// <summary>
			/// Lower date limit for outbound departure accordingly to given date range
			/// </summary>
			public DateTime MinOutDate { get; init; }
			/// <summary>
			/// Upper date limit for outbound departure accordingly to given date range
			/// </summary>
			public DateTime MaxOutDate { get; init; }
			/// <summary>
			/// Lower date limit for return departure accordingly to given date range
			/// </summary>
			public DateTime MinReturnDate { get; init; }
			/// <summary>
			/// Upper date limit for return departure accordingly to given date range
			/// </summary>
			public DateTime MaxReturnDate { get; init; }

			public Parameters(FlightQuery query, FlightDbOptions options)
			{
				decimal maxPriceRub = (query.SearchCriteria?.MaxPrice ?? 0) * options.UsdToRubConversionRate;
				MaxPriceRub = maxPriceRub != 0 ? maxPriceRub : decimal.MaxValue;
				MaxFlightOffers = query.SearchCriteria?.MaxFlightOffers ?? int.MaxValue;
				MinOutDate = query.DepartureDate?.MinDate ?? DateTime.MinValue; //Test query without a date
				MaxOutDate = query.DepartureDate?.MaxDate ?? DateTime.MaxValue;
				MinReturnDate = query.ReturnDate?.MinDate ?? DateTime.MinValue;
				MaxReturnDate = query.ReturnDate?.MaxDate ?? DateTime.MaxValue;
			}
		}
		#endregion
		#endregion
	}
}
