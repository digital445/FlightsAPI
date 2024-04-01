using AutoMapper;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.FlightDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using System;
using System.Linq.Expressions;

namespace FlightsAPI.Infrastructure.DataBases
{
	public class FlightRepository(
		FlightDbContext DbContext,
		IOptions<FlightDbOptions> Options,
		IMapper Mapper) : IFlightRepository
	{
		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			//TODO: refactor this monstrous method clean code way

			//Get search parameters

			DateTime? minOutDate = query.DepartureDate?.MinDate;
			DateTime? maxOutDate = query.DepartureDate?.MaxDate;
			DateTime? minReturnDate = query.ReturnDate?.MinDate;
			DateTime? maxReturnDate = query.ReturnDate?.MaxDate;

			SearchCriteria? sc = query.SearchCriteria;

			decimal maxPriceRub = sc?.MaxPrice * Options.Value.UsdToRubConversionRate ?? 0;
			maxPriceRub = maxPriceRub == 0 ? decimal.MaxValue : maxPriceRub;

			int maxFlightOffers = sc?.MaxFlightOffers ?? int.MaxValue;


			//Construct filters

			Expression<Func<Flight, bool>> basicOutFilter = fl =>
				fl.DepartureAirport == query.OriginLocationCode &&
				fl.ArrivalAirport == query.DestinationLocationCode &&
				(!minOutDate.HasValue || fl.ScheduledDeparture.UtcDateTime.Date >= minOutDate.Value) &&
				(!maxOutDate.HasValue || fl.ScheduledDeparture.UtcDateTime.Date <= maxOutDate.Value) &&
				fl.TicketFlights.Any(); //skip flights with tickets unavailable;

			Expression<Func<Flight, bool>> airlinesFilter = fl =>
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

			//Retrieve outbound flights

			IQueryable<Flight> outFlightsQ = DbContext.Flights
				.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= maxPriceRub))
				.Where(CombineExpressions(basicOutFilter, airlinesFilter))
				.Take(maxFlightOffers);

			await outFlightsQ.ForEachAsync(fl => fl.TicketFlights = fl.TicketFlights.Take(1)); //drop all TicketFlights collection items but first

			var outFlights = await outFlightsQ.ToListAsync();


			if (query.ReturnDate == null)
			{
				return Mapper.Map<IEnumerable<FlightOffer>>(outFlights);
			}


			Expression<Func<Flight, bool>> BuildBasicReturnFilter(DateTime minTime) =>
				fl =>
					fl.DepartureAirport == query.DestinationLocationCode &&
					fl.ArrivalAirport == query.OriginLocationCode &&
					(!minReturnDate.HasValue || fl.ScheduledDeparture.UtcDateTime.Date >= minReturnDate.Value) &&
					fl.ScheduledDeparture.UtcDateTime >= minTime && //minReturnDepartTime should be calculated by adding the Minimal Connecting Time to the outbound flight arrival
					(!maxReturnDate.HasValue || fl.ScheduledDeparture.UtcDateTime.Date <= maxReturnDate.Value) &&
					fl.TicketFlights.Any(); //skip flights with tickets unavailable;


			var allFlights = outFlights.SelectMany(outFlight =>
			{
				var minReturnDepartTime = outFlight.ScheduledArrival.UtcDateTime.AddMinutes(Options.Value.MCT);

				//Retrieve return flights for the given out flight
				var returnFlightsQ = DbContext.Flights
					.Include(fl => fl.TicketFlights.Where(tf => tf.Amount <= maxPriceRub))
					.Where(CombineExpressions
					(
						BuildBasicReturnFilter(minReturnDepartTime),
						airlinesFilter
					))
					.Take(Options.Value.MaxReturnFlights);
				var returnFlights = returnFlightsQ.ToList();
				returnFlights.ForEach(fl => fl.TicketFlights = fl.TicketFlights.Take(1));
				return returnFlights.Select(retFlight => (OutFlight: outFlight, RetFlight: retFlight));
			}).Where(twoWay => twoWay.RetFlight != null); //drop combinations without return flights

			return Mapper.Map<IEnumerable<FlightOffer>>(allFlights);
		}
		public Task<BookingResult> BookFlights(BookingOrder query)
		{
			throw new NotImplementedException();
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
	}
}
