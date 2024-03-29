using AutoMapper;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	/// <summary>
	/// Adapts IAmadeusClient to IFlightService
	/// </summary>
	public class AmadeusAdapter(
		IAmadeusClient AmadeusClient, 
		IMapper Mapper) : IAmadeusAdapter
	{

		public async Task<IEnumerable<FlightOffer>> GetFlightOffers(FlightQuery query)
		{
			AmadeusFlightQuery amadeusQuery = Mapper.Map<AmadeusFlightQuery>(query);

			IEnumerable<AmadeusFlightOffer> amadeusOffers = await AmadeusClient.GetFlightOffers(amadeusQuery);

			return Mapper.Map<IEnumerable<FlightOffer>>(amadeusOffers);
		}
		public async Task<BookingResult> BookFlights(BookingOrder query)
		{
			if (IsValidationFailed(query, out IEnumerable<OrderIssue>? issues))
			{
				return new BookingResult { Issues = issues };
			}

			AmadeusBookingOrder amadeusOrder = Mapper.Map<AmadeusBookingOrder>(query);
			AmadeusBookingQuery amadeusQuery = new() { Data = amadeusOrder };

			AmadeusBookingOrder amadeusOrderRes = await AmadeusClient.BookFlights(amadeusQuery);

			var result = Mapper.Map<BookingResult>(amadeusOrderRes);

			return result;
		}

		private bool IsValidationFailed(BookingOrder query, out IEnumerable<OrderIssue>? issues)
		{
			if (query.FlightOffer == null)
			{
				issues = [new OrderIssue { 
					Title = "FlightOffer is empty.", 
					Detail = "Pass an appropriate FlightOffer." 
				}];
				return true;
			}
			if (query.FlightOffer.FlightProvider != FlightProvider.Amadeus)
			{
				issues = [new OrderIssue {
					Title = "Wrong FlightProvider",
					Detail = "FlightOffer does not correspond to the flight provider Amadeus."
				}];
				return true;
			}
			issues = null;
			return false;
		}
	}
}
