namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusFlightOfferQuery
	{
		public string? CurrencyCode { get; init; }
		public OriginDestination[]? OriginDestinations { get; init; }
		public ExtendedTravelInfo[]? Travelers { get; init; }
		public SearchCriteria SearchCriteria { get; init; } = new();
		public string[] Sources { get; } = ["GDS"];
	};

	public record OriginDestination
	{
		public string Id { get; init; } = "1";
		public string? OriginLocationCode { get; init; }
		public string? DestinationLocationCode { get; init; }
		public DateTimeRange? DepartureDateTimeRange { get; init; }
		public DateTimeRange? ArrivalDateTimeRange { get; init; }
	}

	public record DateTimeRange
	{
		public string? Date { get; init; }
		public string? DateWindow { get; init; }
		public string? Time { get; init; }
		public string? TimeWindow { get; init; }
	}

	public record ExtendedTravelInfo
	{
		public string Id { get; init; } = "1";
		public string TravelerType { get; } = "ADULT"; //as the FlighsAPI does not distinct age, use the default "ADULT" value for external API
	};

	public record SearchCriteria
	{
		public bool AddOnewayOffers { get; init; } = true;
		public int? MaxPrice { get; init; }
		public FlightFilters? FlightFilters { get; init; }
	}

	public record FlightFilters
	{
		public CarrierRestrictions? CarrierRestrictions { get; init; }
		public ConnectionRestriction? ConnectionRestriction { get; init; }
	}

	public record CarrierRestrictions
	{
		public string[]? ExcludedCarrierCodes { get; init; }
		public string[]? IncludedCarrierCodes { get; init; }
	}

	public record ConnectionRestriction
	{
		public int MaxNumberOfConnections { get; init; }
	}
}