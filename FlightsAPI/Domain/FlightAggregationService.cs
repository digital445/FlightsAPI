using FlightsAPI.Domain.Interfaces;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using static FlightsAPI.Enumerations;

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

			var sorted = SortByCriteria(flightOffers, query.SortCriteria);
			return sorted;
		}

		public async Task<BookingResult> BookFlights(BookingOrder query)
		{
			return query.FlightOffer?.FlightProvider switch
			{
				FlightProvider.Amadeus => await AmadeusAdapter.BookFlights(query),
				FlightProvider.DemoDB => await FlightRepository.BookFlights(query),
				_ => new BookingResult { Issues = [new OrderIssue { Title = "Unknown flight provider.", Detail = "Unknown flight provider or FlightOffer is null." }]}
			};
			
		}

		private List<FlightOffer> SortByCriteria(List<FlightOffer> list, SortCriteria criteria)
		{
			IEnumerable<FlightOffer> ascResult = criteria.Criteria switch
			{
				SortBy.Price => list.OrderBy(fo => fo.Price?.Total),
				SortBy.OutboundDeparture => list.OrderBy(GetOutboundDepartureTime),
				SortBy.InboundDeparture => list.OrderBy(GetInboundDepartureTime),
				SortBy.ConnectionNumber => list.OrderBy(CountConnections),
				_ => list
			};

			return criteria.Criteria == SortBy.None || criteria.Order == SortOrder.Ascending
				? ascResult.ToList() 
				: ascResult.Reverse().ToList();
		}

		private DateTime? GetOutboundDepartureTime(FlightOffer flightOffer) =>
			flightOffer.Itineraries?.Length > 0 && flightOffer.Itineraries[0].Segments?.Length > 0
				? flightOffer.Itineraries[0].Segments?[0].Departure?.At
				: null;
		private DateTime? GetInboundDepartureTime(FlightOffer flightOffer) =>
			flightOffer.Itineraries?.Length > 1 && flightOffer.Itineraries[1].Segments?.Length > 0
				? flightOffer.Itineraries[1].Segments?[0].Departure?.At
				: null;
		private int CountConnections(FlightOffer flightOffer) =>
			flightOffer.Itineraries?.Sum(it => it.Segments?.Length ?? 0) ?? 0;
	}
}
