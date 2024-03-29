using AutoMapper;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;

namespace FlightsAPI.MapperProfiles
{
	public class BookingResultProfile : Profile
	{
		public BookingResultProfile() 
		{
			CreateMap<AmadeusBookingOrder, BookingResult>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => RemoveTrailingSubstring(src.Id, "%3D")));
			CreateMap<AmAssosiatedRecord, AssosiatedRecord>();
		}

		private static string RemoveTrailingSubstring(string? input, string? substring)
		{
			ArgumentNullException.ThrowIfNull(input);
			ArgumentNullException.ThrowIfNull(substring);

			if (input.EndsWith(substring))
			{
				return input.Remove(input.Length - substring.Length);
			}
			return input;
		}
	}
}
