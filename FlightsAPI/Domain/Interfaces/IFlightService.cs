using FlightsAPI.Models;

namespace FlightsAPI.Domain.Interfaces
{
    public interface IFlightService : IFlightSearch<FlightQuery, FlightOffer>, IFlightBook<BookingOrder, BookingOrder>
    {
    }
}
