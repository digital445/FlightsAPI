using AutoMapper;
using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	/// <summary>
	/// Adapts IAmadeusClient to IFlightService
	/// </summary>
	public class AmadeusAdapter(IAmadeusClient AmadeusClient, IMapper Mapper) : IFlightService
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

			return Mapper.Map<IEnumerable<FlightOffer>>(amadeusOffers);
		}
	}
}
