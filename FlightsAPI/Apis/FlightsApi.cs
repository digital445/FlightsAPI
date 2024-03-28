using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FlightsAPI.Apis
{
	public static class FlightsApi
	{
		public static IEndpointRouteBuilder MapFlightsApi(this IEndpointRouteBuilder app)
		{
			app.MapPost("/flight-offers", GetFlightOffers);

			return app;
		}

		public static async Task<Results<Ok<IEnumerable<FlightOffer>>, IResult>> GetFlightOffers(
			FlightQuery query, 
			IFlightService flightService)
		{

			var flightOffers = await flightService.GetFlightOffers(query);
			if (flightOffers.Any())
			{ 
				return TypedResults.Ok(flightOffers);
			}
			return TypedResults.NoContent();
		}

	}
}
