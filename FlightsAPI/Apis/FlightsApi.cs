using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FlightsAPI.Apis
{
	public static class FlightsApi
	{
		public static IEndpointRouteBuilder MapFlightsApi(this IEndpointRouteBuilder app)
		{
			app.MapPost("/search", GetFlightOffers);
			app.MapPost("/book", BookFlights);

			return app;
		}

		private static async Task<Results<Ok<IEnumerable<FlightOffer>>, IResult>> GetFlightOffers(
			FlightQuery query, 
			IFlightService flightService)
		{
			ValidateQuery(query);

			var flightOffers = await flightService.GetFlightOffers(query);
			if (flightOffers.Any())
			{
				return TypedResults.Ok(flightOffers);
			}
			return TypedResults.NoContent();
		}


		private static async Task<Results<Ok<IEnumerable<FlightOffer>>, IResult>> BookFlights(
			BookingOrder query, 
			IFlightService flightService)
		{

			var bookingOrder = await flightService.BookFlights(query);
			return TypedResults.Ok(bookingOrder);
		}

		
		private static void ValidateQuery(FlightQuery query)
		{
			ArgumentNullException.ThrowIfNull(query);
			ArgumentNullException.ThrowIfNull(query.DepartureDate);
			if (query.ReturnDate != null &&
				query.ReturnDate.Date < query.DepartureDate.Date)
			{
				throw new ArgumentException("The return flight should be after the outbound flight.");
			}
		}

	}
}
