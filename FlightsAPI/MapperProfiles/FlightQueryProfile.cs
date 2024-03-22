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
				.ForMember(dest => dest.OriginDestinations, opt => opt.MapFrom(src => new[] {
					new OriginDestination { //flight
						Id = "1",
						OriginLocationCode = src.OriginLocationCode,
						DestinationLocationCode = src.DestinationLocationCode,
						DepartureDateTimeRange = new DateTimeRange {
							Date = src.DepartureDate.Date, 
							DateWindow = src.DepartureDate.DateWindow
						}
					},
					new OriginDestination { //return flight
						Id = "2",
						OriginLocationCode = src.DestinationLocationCode,
						DestinationLocationCode = src.OriginLocationCode,
						DepartureDateTimeRange = new DateTimeRange {
							Date = src.ReturnDate.Date,
							DateWindow = src.ReturnDate.DateWindow
						}
					}
				}))
				.ForMember(dest => dest.Travelers, opt => opt.MapFrom(src => 
					Enumerable.Range(1, src.PassengerAmount)
						.Select(i => new ExtendedTravelInfo { Id = i.ToString() }) 
						.ToArray()
				));
		}
	}
}
