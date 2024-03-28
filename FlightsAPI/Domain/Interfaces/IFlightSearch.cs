using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.Domain.Interfaces
{
    public interface IFlightSearch<in TQuery, TResult>
    {
		Task<IEnumerable<TResult>> GetFlightOffers(TQuery query);
    }
}
