using AutoMapper;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.MapperProfiles
{
	public class BookingQueryProfile : Profile
	{
		public BookingQueryProfile()
		{
			CreateMap<BookingQuery, AmadeusBookingQuery>()
				.ConstructUsing(MapQueryToAmQuery);
			CreateMap<TravelerInfo, AmTravelerInfo>();
			CreateMap<TravelerName, AmTravelerName>();
			CreateMap<ContactInfo, AmContactInfo>()
				.ForMember(dest => dest.Phones, opt => opt.MapFrom(src => MapPhoneInfoToArray(src)));
		}
        private static AmPhoneInfo[] MapPhoneInfoToArray(ContactInfo src)
		{
			return [new() {
						DeviceType = src.PhoneType,
						CountryCallingCode = src.CountryCallingCode,
						Number = src.Number
					}];
		}
		private static AmadeusBookingQuery MapQueryToAmQuery(BookingQuery src, ResolutionContext context)
		{
			return new AmadeusBookingQuery
			{
				Data = new OrderData
				{
					FlightOffers = [context.Mapper.Map<AmadeusFlightOffer>(src.FlightOffer)],
					Travelers = [context.Mapper.Map<AmTravelerInfo>(src.Traveler)]
				}
			};
		}
	}
}
