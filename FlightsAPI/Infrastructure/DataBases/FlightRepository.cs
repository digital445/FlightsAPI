using AutoMapper;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.FlightDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq.Expressions;

namespace FlightsAPI.Infrastructure.DataBases
{
	public class FlightRepository(
		FlightDbContext DbContext,
		IOptions<FlightDbOptions> Options,
		IMapper Mapper) : IFlightRepository
	{
		public Task<BookingResult> BookFlights(BookingOrder query)
		{
			throw new NotImplementedException();
		}


		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			var parameters = new Parameters(query, Options.Value);

			List<Flight> outFlights = await RetrieveOutboundFlightsAsync(query, parameters);

			if (query.ReturnDate == null)
				return MapToFlightOffers(outFlights);

			var flightCombinations = await RetrieveFlightCombinationsAsync(outFlights, query, parameters);

			var flightOffers = MapToFlightOffers(flightCombinations);
			SetFlightOfferIds(flightOffers);

			return flightOffers;
		}


		#region Private
		private async Task<IEnumerable<(Flight[] outSegments, Flight[] retSegments)>> RetrieveFlightCombinationsAsync(List<Flight> outFlights, FlightQuery query, Parameters parameters)
		{
			var tasks = outFlights.Select(async outFlight =>
			{
				var returnFlights = await RetrieveReturnFlightsAsync(outFlight, query, parameters);
				return AssembleCombinations(outFlight, returnFlights);
			});

			return await RetrieveCombinationsConsecutivelyAsync(tasks, parameters);
		}

		private static async Task<IEnumerable<(Flight[], Flight[])>> RetrieveCombinationsConsecutivelyAsync(IEnumerable<Task<(Flight[], Flight[])[]>> tasks, Parameters parameters)
		{
			//Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance.
			//https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues
			//So use consecutive way

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
			return combinations;
		}

		/// <summary>
		/// Produce the Array of combinations related to the given outflight. Combination is a tuple of flight arrays, each array representing a set of one-way flight segments
		/// </summary>
		private static (Flight[] outSegments, Flight[] retSegments)[] AssembleCombinations(Flight outFlight, List<Flight> returnFlights) =>
			returnFlights.Select<Flight, (Flight[], Flight[])>(retFlight => ([outFlight], [retFlight])).ToArray();

		/// <summary>
		/// Retrieve return flights for the given outbound flight
		/// </summary>
		private async Task<List<Flight>> RetrieveReturnFlightsAsync(Flight outFlight, FlightQuery query, Parameters parameters) =>
			await DbContext.Flights
				.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= parameters.MaxPriceRub))
				.Where(CombineExpressions(
					GetBasicReturnFilter(query, outFlight, parameters),
					GetAirlinesFilter(query)))
				.Take(Options.Value.MaxReturnFlights)
				.ToListAsync();

		private IEnumerable<FlightOffer> MapToFlightOffers(List<Flight> outFlights) =>
			Mapper.Map<IEnumerable<FlightOffer>>(outFlights);
		private IEnumerable<FlightOffer> MapToFlightOffers(IEnumerable<(Flight[] outSegments, Flight[] retSegments)> flightCombinations) =>
			Mapper.Map<IEnumerable<FlightOffer>>(flightCombinations);

		/// <summary>
		/// Set Id to each flight offer in flight offer collection
		/// </summary>
		/// <param name="flightOffers"></param>
		private void SetFlightOfferIds(IEnumerable<FlightOffer> flightOffers)
		{
			int i = 1;
			foreach (var fl in flightOffers)
				fl.Id = i++.ToString();
		}

		private async Task<List<Flight>> RetrieveOutboundFlightsAsync(FlightQuery query, Parameters parameters) =>
			await DbContext.Flights
				.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= parameters.MaxPriceRub))
				.Where(CombineExpressions(
					GetBasicOutFliter(query, parameters),
					GetAirlinesFilter(query)))
				.Take(parameters.MaxFlightOffers)
				.ToListAsync();

		private Expression<Func<Flight, bool>> GetBasicOutFliter(FlightQuery query, Parameters parameters) =>
			fl =>
				fl.DepartureAirport == query.OriginLocationCode &&
				fl.ArrivalAirport == query.DestinationLocationCode &&
				fl.ScheduledDeparture.UtcDateTime.Date >= parameters.MinOutDate &&
				fl.ScheduledDeparture.UtcDateTime.Date <= parameters.MaxOutDate &&
				fl.TicketFlights.Any(); //skip flights with tickets unavailable;

		private Expression<Func<Flight, bool>> GetBasicReturnFilter(FlightQuery query, Flight outFlight, Parameters parameters)
		{
			var minReturnDepartTime = outFlight.ScheduledArrival.UtcDateTime.AddMinutes(Options.Value.MCT); //minimal acceptable time between two consequent flights
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
	}
}
