using AutoMapper;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.MapperProfiles
{
	public class FlightOfferProfile : Profile
	{
		public FlightOfferProfile() 
		{
			CreateMap<AmadeusFlightOffer, FlightOffer>();
		}
	}
}
 