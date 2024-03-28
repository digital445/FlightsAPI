using AutoMapper;
using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	/// <summary>
	/// Adapts IAmadeusClient to IFlightService
	/// </summary>
	public class AmadeusAdapter(
		IAmadeusClient AmadeusClient, 
		IMapper Mapper,
		IOptions<JsonSerializerOptions> JOptions) : IFlightService
	{
		public async Task<BookingResult> BookFlights(BookingQuery query)
		{
			AmadeusBookingQuery amadeusQuery = Mapper.Map<AmadeusBookingQuery>(query);

			AmadeusBookingResult amadeusResult = await AmadeusClient.BookFlights(amadeusQuery);

			return Mapper.Map<BookingResult>(amadeusResult);
		}

		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			AmadeusFlightQuery amadeusQuery = Mapper.Map<AmadeusFlightQuery>(query);

			IEnumerable<AmadeusFlightOffer> amadeusOffers = await AmadeusClient.GetFlightOffers(amadeusQuery);

			string json = "{\"data\":{\"type\":\"flight-order\",\"flightOffers\":[],\"travelers\":[{\"id\":\"1\",\"dateOfBirth\":\"1982-01-16\",\"name\":{\"firstName\":\"JORGE\",\"lastName\":\"GONZALES\"},\"gender\":\"MALE\",\"contact\":{\"emailAddress\":\"jorge.gonzales833@telefonica.es\",\"phones\":[{\"deviceType\":\"MOBILE\",\"countryCallingCode\":\"34\",\"number\":\"480080076\"}]}}]}}";
			AmadeusBookingQuery? bookingQuery = JsonSerializer.Deserialize<AmadeusBookingQuery>(json, JOptions.Value);
			if (bookingQuery?.Data == null)
			{
				return [];
			}
			bookingQuery.Data.FlightOffers[0] = amadeusOffers.FirstOrDefault();
			AmadeusBookingResult amadeusResult = await AmadeusClient.BookFlights(bookingQuery);



			return Mapper.Map<IEnumerable<FlightOffer>>(amadeusOffers);
		}
	}
}
