using AutoMapper;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.FlightDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

			Flight[][] outRoutes = await RetrieveOutboundRoutesAsync(query, parameters);

			if (query.ReturnDate == null)
				return MapToFlightOffers(outRoutes);

			var routeCombinations = await RetrieveRouteCombinationsAsync(outRoutes, query, parameters);

			return MapToFlightOffers(routeCombinations); ;
		}


		#region Private
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
				.Select(fl => new Flight[] {fl})
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
				.Take(Options.Value.MaxReturnFlights)
				.Select(fl => new Flight[] {fl})
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
			var minReturnDepartTime = outRoute[^1].ScheduledArrival.UtcDateTime.AddMinutes(Options.Value.MCT); //minimal acceptable time between two consequent flights
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
