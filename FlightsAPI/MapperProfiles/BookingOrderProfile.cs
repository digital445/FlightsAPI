using AutoMapper;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.MapperProfiles
{
	public class BookingOrderProfile : Profile
	{
		public BookingOrderProfile()
		{
			CreateMap<BookingOrder, AmadeusBookingOrder>()
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
		private static AmadeusBookingOrder MapQueryToAmQuery(BookingOrder src, ResolutionContext context)
		{
			return new AmadeusBookingOrder
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
