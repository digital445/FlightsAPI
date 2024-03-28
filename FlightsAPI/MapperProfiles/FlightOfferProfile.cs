using AutoMapper;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using System.Xml;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.MapperProfiles
{
	public class FlightOfferProfile : Profile
	{
		public FlightOfferProfile() 
		{
			CreateMap<AmadeusFlightOffer, FlightOffer>()
				.ForMember(dest => dest.FlightProvider, opt => opt.MapFrom(src => FlightProvider.Amadeus));
			CreateMap<AmItinerary, Itinerary>()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToTimeSpan(src.Duration)));
			CreateMap<AmSegment, Segment>()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToTimeSpan(src.Duration)));
			CreateMap<AmAirport, Airport>();
			CreateMap<AmAircraft, Aircraft>();
			CreateMap<AmPrice, Price>();
			CreateMap<AmTravelerPricing, TravelerPricing>();
			CreateMap<AmFareDetailsBySegment, FareDetailsBySegment>();
		}

		private static TimeSpan? ConvertToTimeSpan(string? duration)
		{
			return duration != null ? XmlConvert.ToTimeSpan(duration) : null;
		}
	}
}
 