namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusFlightOffer
	{
		public string Type { get; } = "flight-offer";
		public string? Id { get; init; }
		public string Source { get; } = "GDS";
		public bool DisablePricing { get; } = true; //allows to skip the pricing step
		public AmItinerary[]? Itineraries { get; init; }
		public AmPrice? Price { get; init; }
        public string[]? ValidatingAirlineCodes { get; init; }
        public AmTravelerPricing[]? TravelerPricings { get; init; }
    }

	/// <summary>
	/// Avia Route
	/// </summary>
	public record AmItinerary
	{
		public string? Duration { get; init; } // ISO 8601 format (PnYnMnDTnHnMnS)
		public AmSegment[]? Segments { get; init; }
	}

	/// <summary>
	/// Point-to-point flight
	/// </summary>
	public record AmSegment
	{
		public string? Id { get; init; }
		public AmAirport? Departure { get; init; }
		public AmAirport? Arrival { get; init; }
		public string? CarrierCode { get; init; }
		public string? Number { get; init; }
		public AmAircraft? Aircraft { get; init; }
		public string? Duration { get; init; } // ISO 8601 format (PnYnMnDTnHnMnS)
	}

	public record AmAirport
	{
		public string? IataCode { get; init; }
		public string? At { get; init; }
	}

	public record AmAircraft
	{
		public string? Code { get; init; }
	}
	public record AmPrice
	{
		public string? Currency { get; init; }
		public string? Total { get; init; }
	}
	public record AmTravelerPricing
	{
		public string? TravelerId { get; init; }
		public string? FareOption { get; init; }
		public string? TravelerType { get; init; }
		public AmFareDetailsBySegment[]? FareDetailsBySegment { get; init; }
	}

	public record AmFareDetailsBySegment
	{
		public string? SegmentId { get; init; }
		public string? Class { get; init; }
		public string? Cabin { get; init; }
	}
}
