using FlightsAPI.Models.Amadeus;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.Models
{
	public record FlightOffer
	{
		public string? Id { get; init; }
		public FlightProvider FlightProvider { get; init; }
		public Itinerary[]? Itineraries { get; init; }
		public Price? Price { get; init; }
		public string[]? ValidatingAirlineCodes { get; init; }
		public TravelerPricing[]? TravelerPricings { get; init; }

	}

	/// <summary>
	/// Avia Route
	/// </summary>
	public record Itinerary
	{
		public TimeSpan? Duration { get; init; }
		public Segment[]? Segments { get; init; }
	}

	/// <summary>
	/// Point-to-point flight
	/// </summary>
	public record Segment
	{
		public string? Id { get; init; }
		public Airport? Departure { get; init; }
		public Airport? Arrival { get; init; }
		public string? CarrierCode { get; init; }
		public string? Number { get; init; }
		public Aircraft? Aircraft { get; init; }
		public TimeSpan? Duration { get; init; } // ISO 8601 format (PnYnMnDTnHnMnS)
	}

	public record Airport
	{
		public string? IataCode { get; init; }
		public DateTime At { get; init; }
	}
	public record Aircraft
	{
		public string? Code { get; init; }
	}

	public record Price
	{
		public string? Currency { get; init; }
		public string? Total { get; init; }
	}
	public record TravelerPricing
	{
		public string? TravelerId { get; init; }
		public string? FareOption { get; init; }
		public string? TravelerType { get; init; }
		public FareDetailsBySegment[]? FareDetailsBySegment { get; init; }
	}

	public record FareDetailsBySegment
	{
		public string? SegmentId { get; init; }
		public string? Class { get; init; }
	}
}
