using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.Infrastructure.ExternalApis.Interfaces
{
	public interface IAmadeusClient : IFlightSearch<AmadeusFlightQuery, AmadeusFlightOffer>, IFlightBook<AmadeusBookingQuery, AmadeusBookingOrder>
	{
	}
}
