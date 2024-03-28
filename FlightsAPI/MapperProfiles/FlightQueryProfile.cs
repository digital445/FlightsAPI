using AutoMapper;
using FlightsAPI.Models.Amadeus;
using FlightsAPI.Models;

namespace FlightsAPI.MapperProfiles
{
	public class FlightQueryProfile : Profile
	{
		public FlightQueryProfile()
		{
			CreateMap<FlightQuery, AmadeusFlightQuery>()
				.ForMember(dest => dest.OriginDestinations, opt => opt.MapFrom(src => CreateOriginDestinations(src)));
			CreateMap<SearchCriteria, AmSearchCriteria>()
				.ForMember(dest => dest.FlightFilters, opt => opt.MapFrom(src => CreateFlightFilters(src)));
		}

		private static AmOriginDestination[] CreateOriginDestinations(FlightQuery src)
		{
			AmOriginDestination[] array = new AmOriginDestination[src.ReturnDate.Date == null ? 1 : 2];
			array[0] = new AmOriginDestination //outgoing flight
			{
				Id = "1",
				OriginLocationCode = src.OriginLocationCode,
				DestinationLocationCode = src.DestinationLocationCode,
				DepartureDateTimeRange = new AmDateTimeRange
				{
					Date = src.DepartureDate.Date,
					DateWindow = src.DepartureDate.DateWindow
				}
			};
			if (array.Length == 2)
			{
				array[1] = new AmOriginDestination //return flight
				{
					Id = "2",
					OriginLocationCode = src.DestinationLocationCode,
					DestinationLocationCode = src.OriginLocationCode,
					DepartureDateTimeRange = new AmDateTimeRange
					{
						Date = src.ReturnDate.Date,
						DateWindow = src.ReturnDate.DateWindow
					}
				};
			}
			return array;
		}
		private static AmFlightFilters CreateFlightFilters(SearchCriteria srcCriteria) =>
			new()
			{
				CarrierRestrictions = new() { 
					ExcludedCarrierCodes = srcCriteria.ExcludedCarrierCodes,
					IncludedCarrierCodes = srcCriteria.IncludedCarrierCodes
				},
				ConnectionRestriction = new()
				{
					MaxNumberOfConnections = srcCriteria.MaxNumberOfConnections
				}
			};
	}
}
