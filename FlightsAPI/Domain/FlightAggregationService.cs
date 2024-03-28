using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Domain
{
    public class FlightAggregationService(IAmadeusAdapter AmadeusAdapter, IFlightRepository FlightRepository) : IFlightService
	{
		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			IEnumerable<FlightOffer> amadeusFlightOffers = await AmadeusAdapter.GetFlightOffers(query);
			IEnumerable<FlightOffer> dbFlightOffers = await FlightRepository.GetFlightOffers(query);

			List<FlightOffer> flightOffers = [];
			flightOffers.AddRange(amadeusFlightOffers);
			flightOffers.AddRange(dbFlightOffers);

			return flightOffers;
		}
		public Task<BookingResult> BookFlights(BookingQuery query)
		{
			throw new NotImplementedException();
		}
	}
}
