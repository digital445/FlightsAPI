using AutoMapper;
using FlightsAPI.Infrastructure.DataBases;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using FlightsAPI.Models.FlightDb;
using Microsoft.Extensions.Options;
using System.Globalization;
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

			//Database Flights to FlightOffer
			CreateMap<Flight[], FlightOffer>() //use this mapping for oneWay FlightOffers only
				.ConstructUsing(outRoute => CreateFlightOffer(outRoute, null));
			CreateMap<(Flight[] outRoute, Flight[] retRoute), FlightOffer>()
				.ConstructUsing(twoWay => CreateFlightOffer(twoWay.outRoute, twoWay.retRoute));
		}

		private FlightOffer CreateFlightOffer(Flight[] outRoute, Flight[]? retRoute) =>
			new FlightOffer
			{
				FlightProvider = FlightProvider.DemoDB,
				Itineraries = CreateItineraries(outRoute, retRoute),
				Price = new Price
				{
					Currency = "USD",
					Total = GetTotalPrice(outRoute, retRoute)
				},
				TravelerPricings = [new TravelerPricing
				{
					TravelerId = "1",
					TravelerType = "ADULT",
					FareOption = "STANDARD",
					FareDetailsBySegment = ExtractFareDetails(outRoute, retRoute)
				}]
			};

		private static Itinerary[] CreateItineraries(Flight[] outSegments, Flight[]? retSegments)
		{
			var itineraries = new Itinerary[retSegments == null ? 1 : 2];

			itineraries[0] = new Itinerary
			{
				Segments = CreateSegments(outSegments),
				Duration = CalculateItineraryDuration(outSegments)
			};

			if (retSegments == null)
				return itineraries;

			itineraries[1] = new Itinerary
			{
				Segments = CreateSegments(retSegments),
				Duration = CalculateItineraryDuration(retSegments)
			};
			return itineraries;
		}

		private static FareDetailsBySegment[] ExtractFareDetails(Flight[] outSegments, Flight[]? retSegments)
		{
			var fareDetails = GetOneWayFareDetails(outSegments);
			if (retSegments != null)
			{
				fareDetails = fareDetails.Concat(GetOneWayFareDetails(retSegments));
			}
			return fareDetails.ToArray();
		}

		private static IEnumerable<FareDetailsBySegment> GetOneWayFareDetails(Flight[] segments) =>
			segments.Select(fl => new FareDetailsBySegment
			{
				SegmentId = fl.FlightId.ToString(),
				Price = fl.TicketFlights.FirstOrDefault()?.Amount,
				Cabin = GetFareConditions(fl)
			});

		private static string? GetFareConditions(Flight fl) =>
			fl.TicketFlights.FirstOrDefault()?.FareConditions;

		private decimal? GetTotalPrice(Flight[] outSegments, Flight[]? retSegments) =>
			GetOneWayTotalPrice(outSegments) 
			+ 
			(retSegments == null ? 0 : GetOneWayTotalPrice(retSegments));
		private decimal? GetOneWayTotalPrice(Flight[] segments) =>
			segments.Aggregate(0m, (acc, fl) => acc + fl.TicketFlights.FirstOrDefault()?.Amount ?? 0)
				/ _flightDbOptions.UsdToRubConversionRate;

		private static Segment[] CreateSegments(Flight[] segments) =>
			segments.Select(fl =>
				new Segment
				{
					Id = fl.FlightId.ToString(),
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
