using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Models;

namespace FlightsAPI.Infrastructure.DataBases
{
    public class FlightRepository(FlightDbContext dbContext) : IFlightService
	{
		public Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			throw new NotImplementedException();
		}
		public Task<BookingOrder> BookFlights(BookingOrder query)
		{
			throw new NotImplementedException();
		}

	}
}
