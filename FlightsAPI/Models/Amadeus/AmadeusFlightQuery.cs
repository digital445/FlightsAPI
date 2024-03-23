namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusFlightQuery
	{
		public string? CurrencyCode { get; init; }
		public AmOriginDestination[]? OriginDestinations { get; init; }
		public AmTravelerInfo[]? Travelers { get; init; }
		public AmSearchCriteria? SearchCriteria { get; init; }
		public string[] Sources { get; init; } = ["GDS"];
	};

	public record AmOriginDestination
	{
		public string Id { get; init; } = "1";
		public string? OriginLocationCode { get; init; }
		public string? DestinationLocationCode { get; init; }
		public AmDateTimeRange? DepartureDateTimeRange { get; init; }
		public AmDateTimeRange? ArrivalDateTimeRange { get; init; }
	}

	public record AmDateTimeRange
	{
		public string? Date { get; init; }
		public string? DateWindow { get; init; }
		public string? Time { get; init; }
		public string? TimeWindow { get; init; }
	}

	public record AmTravelerInfo
	{
		public string Id { get; init; } = "1";
		public string TravelerType { get; init; } = "ADULT"; //as the FlighsAPI does not distinct age, use the default "ADULT" value for external API
	};

	public record AmSearchCriteria
	{
		public int? MaxFlightOffers { get; init; }
        public int? MaxPrice { get; init; }
		public AmFlightFilters? FlightFilters { get; init; }
	}

	public record AmFlightFilters
	{
		public AmCarrierRestrictions? CarrierRestrictions { get; init; }
		public AmConnectionRestriction? ConnectionRestriction { get; init; }
	}

	public record AmCarrierRestrictions
	{
		public string[]? ExcludedCarrierCodes { get; init; }
		public string[]? IncludedCarrierCodes { get; init; }
	}

	public record AmConnectionRestriction
	{
		public int? MaxNumberOfConnections { get; init; }
	}
}