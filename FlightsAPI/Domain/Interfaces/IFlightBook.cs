namespace FlightsAPI.Domain.Interfaces
{
    public interface IFlightBook<in TQuery, TResult>
    {
        Task<TResult> BookFlights(TQuery query);
    }
}
