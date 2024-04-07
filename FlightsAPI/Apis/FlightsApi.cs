using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FlightsAPI.Apis
{
	public static class FlightsApi
	{
		public static IEndpointRouteBuilder MapFlightsApi(this IEndpointRouteBuilder app)
		{
			app.MapPost("/search", GetFlightOffers).CacheOutput("CachePost");
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


		private static async Task<Results<Ok<BookingResult>, IResult>> BookFlights(
			BookingOrder query, 
			IFlightService flightService)
		{

			var bookingResult = await flightService.BookFlights(query);

			Results<Ok<BookingResult>, IResult> result = bookingResult.Issues switch
			{
				null => TypedResults.Ok(bookingResult),
				_ => TypedResults.BadRequest(bookingResult.Issues)
			};
			return result;
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
