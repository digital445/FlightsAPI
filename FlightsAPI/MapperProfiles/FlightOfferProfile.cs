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
				.ForMember(dest => dest.FlightProvider, opt => opt.MapFrom(src => FlightProvider.Amadeus))
				.ReverseMap();
			CreateMap<AmItinerary, Itinerary>()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToTimeSpan(src.Duration)))
				.ReverseMap()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToString(src.Duration)));
			CreateMap<AmSegment, Segment>()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToTimeSpan(src.Duration)))
				.ReverseMap()
				.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertToString(src.Duration)));
			CreateMap<AmAirport, Airport>()
				.ReverseMap()
				.ForMember(dest => dest.At, opt => opt.MapFrom(src => ConvertToString(src.At)));
			CreateMap<AmAircraft, Aircraft>().ReverseMap();
			CreateMap<AmPrice, Price>().ReverseMap();
			CreateMap<AmTravelerPricing, TravelerPricing>().ReverseMap();
			CreateMap<AmFareDetailsBySegment, FareDetailsBySegment>().ReverseMap();
		}

		private static TimeSpan? ConvertToTimeSpan(string? duration)
		{
			return duration != null ? XmlConvert.ToTimeSpan(duration) : null;
		}
		private static string? ConvertToString(TimeSpan? duration)
		{
			return duration != null ? XmlConvert.ToString(duration.Value) : null;
		}
		private static string ConvertToString(DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
		}
	}
}
