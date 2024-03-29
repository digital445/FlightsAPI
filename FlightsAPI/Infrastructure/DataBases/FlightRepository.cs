using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Infrastructure.DataBases
{
    public class FlightRepository() : IFlightRepository
	{
		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			return [];
		}
		public Task<BookingResult> BookFlights(BookingOrder query)
		{
			throw new NotImplementedException();
		}

	}
}
