using AutoMapper;
using FlightsAPI.Infrastructure.DataBases;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using FlightsAPI.Models.FlightDb;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;
using System.Xml;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.MapperProfiles
{
	public class FlightOfferProfile : Profile
	{
		private readonly FlightDbOptions _flightDbOptions;
		public FlightOfferProfile(IOptions<FlightDbOptions> flightDbOptions)
		{
			_flightDbOptions = flightDbOptions.Value;

			//AmadeusFlightOffer to FlightOffer
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
			CreateMap<AmPrice, Price>()
				.ForMember(dest => dest.Total, opt => opt.MapFrom(src => ConvertToDecimal(src.Total)))
				.ReverseMap()
				.ForMember(dest => dest.Total, opt => opt.MapFrom(src => ConvertToString(src.Total)));
			CreateMap<AmTravelerPricing, TravelerPricing>().ReverseMap();
			CreateMap<AmFareDetailsBySegment, FareDetailsBySegment>().ReverseMap();

			//Database Flight to FlightOffer
			CreateMap<Flight[], FlightOffer>() //use this mapping for oneWay FlightOffers only
				.ConstructUsing(outFlights => CreateFlightOffer(outFlights, null));
			CreateMap<(Flight[] outSegments, Flight[] retSegments), FlightOffer>()
				.ConstructUsing(pair => CreateFlightOffer(pair.outSegments, pair.retSegments));
		}

		private FlightOffer CreateFlightOffer(Flight[] outSegments, Flight[]? retSegments) =>
			new FlightOffer
			{
				FlightProvider = FlightProvider.DemoDB,
				Itineraries = CreateItineraries(outSegments, retSegments),
				Price = new Price
				{
					Currency = "USD",
					Total = GetTotalPrice(outSegments, retSegments)
				},
				TravelerPricings = [new TravelerPricing
				{
					TravelerId = "1",
					TravelerType = "ADULT",
					FareOption = "STANDARD",
					FareDetailsBySegment = ExtractFareDetails(outSegments, retSegments)
				}]
			};

		private static Itinerary[] CreateItineraries(Flight[] outFlights, Flight[]? retFlights)
		{
			var itineraries = new Itinerary[retFlights == null ? 1 : 2];

			itineraries[0] = new Itinerary
			{
				Segments = CreateSegments(outFlights, 1),
				Duration = CalculateItineraryDuration(outFlights)
			};

			if (retFlights == null)
				return itineraries;

			itineraries[1] = new Itinerary
			{
				Segments = CreateSegments(retFlights, outFlights.Length + 1),
				Duration = CalculateItineraryDuration(retFlights)
			};
			return itineraries;
		}

		private static FareDetailsBySegment[] ExtractFareDetails(Flight[] outSegments, Flight[]? retSegments)
		{
			var fareDetails = GetOneWayFareDetails(outSegments, 1);
			if (retSegments != null)
			{
				fareDetails = fareDetails.Concat(GetOneWayFareDetails(retSegments, outSegments.Length + 1));
			}
			return fareDetails.ToArray();
		}

		private static IEnumerable<FareDetailsBySegment> GetOneWayFareDetails(Flight[] segments, int startSegmentId) =>
			segments.Select((fl, i) => new FareDetailsBySegment
			{
				SegmentId = (startSegmentId + i).ToString(),
				Cabin = GetFareConditions(fl)
			});

		private static string? GetFareConditions(Flight fl) =>
			fl.TicketFlights.FirstOrDefault()?.FareConditions.ToUpper();

		private decimal? GetTotalPrice(Flight[] outSegments, Flight[]? retSegments) =>
			GetOneWayTotalPrice(outSegments) 
			+ 
			(retSegments == null ? 0 : GetOneWayTotalPrice(outSegments));
		private decimal? GetOneWayTotalPrice(Flight[] segments) =>
			segments.Aggregate(0m, (acc, fl) => acc + fl.TicketFlights.FirstOrDefault()?.Amount ?? 0)
				/ _flightDbOptions.UsdToRubConversionRate;

		private static Segment[] CreateSegments(Flight[] segments, int startSegmentId) =>
			segments.Select((fl, i) =>
				new Segment
				{
					Id = (startSegmentId + i).ToString(),
					Departure = new Airport
					{
						IataCode = fl.DepartureAirport,
						At = fl.ScheduledDeparture.UtcDateTime,
					},
					Arrival = new Airport
					{
						IataCode = fl.ArrivalAirport,
						At = fl.ScheduledArrival.UtcDateTime
					},
					CarrierCode = ExtractCarrierCode(fl.FlightNo),
					Number = ExtractFlightNumber(fl.FlightNo),
					Aircraft = new Aircraft { Code = fl.AircraftCode },
					Duration = CalculateSegmentDuration(fl)
				}
			).ToArray();

		private static TimeSpan CalculateSegmentDuration(Flight fl) =>
			fl.ScheduledArrival - fl.ScheduledDeparture;

		private static TimeSpan CalculateItineraryDuration(Flight[] fls) =>
			fls.Aggregate(TimeSpan.Zero, (acc, fl) => acc + (fl.ScheduledArrival - fl.ScheduledDeparture));

		private static string ExtractCarrierCode(string fullFlightNumber) =>
			fullFlightNumber.Length > 1 ? fullFlightNumber[..2] : "";
		private static string ExtractFlightNumber(string fullFlightNumber) =>
			fullFlightNumber.Length > 2 ? fullFlightNumber[2..] : "";

		private static TimeSpan? ConvertToTimeSpan(string? duration) =>
			duration != null ? XmlConvert.ToTimeSpan(duration) : null;

		private static string? ConvertToString(TimeSpan? duration) =>
			duration != null ? XmlConvert.ToString(duration.Value) : null;

		private static string ConvertToString(DateTime dateTime) =>
			dateTime.ToString("yyyy-MM-ddTHH:mm:ss");

		private static decimal? ConvertToDecimal(string? price) =>
			decimal.TryParse(price, CultureInfo.GetCultureInfo("en-US"), out decimal result)
				? result
				: null;
		private static string? ConvertToString(decimal? price) =>
			price != null
				? ((decimal)price).ToString(CultureInfo.GetCultureInfo("en-US"))
				: null;
	}
}
