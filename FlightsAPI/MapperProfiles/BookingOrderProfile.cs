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
				.ConstructUsing(MapOrderToAmOrder);
			CreateMap<TravelerInfo, AmTravelerInfo>();
			CreateMap<TravelerName, AmTravelerName>();
			CreateMap<ContactInfo, AmContactInfo>()
				.ForMember(dest => dest.Phones, opt => opt.MapFrom(MapPhoneToPhones));
			CreateMap<PhoneInfo, AmPhoneInfo>();
		}

		private AmPhoneInfo[]? MapPhoneToPhones(ContactInfo src, AmContactInfo dest, AmPhoneInfo[]? prop, ResolutionContext context)
		{
			return [context.Mapper.Map<AmPhoneInfo>(src.Phone)];
		}

		private AmadeusBookingOrder MapOrderToAmOrder(BookingOrder src, ResolutionContext context)
		{
			return new AmadeusBookingOrder
			{
				FlightOffers = [context.Mapper.Map<AmadeusFlightOffer>(src.FlightOffer)],
				Travelers = [context.Mapper.Map<AmTravelerInfo>(src.Traveler)]
			};
		}
	}
}
