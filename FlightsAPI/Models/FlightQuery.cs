namespace FlightsAPI.Models
{
	public record FlightQuery
	{
		public string? CurrencyCode { get; init; }


        public string? OriginLocationCode { get; init; }
        public string? DestinationLocationCode { get; init; }

        public DateRange? DepartureDate { get; init; }
        public DateRange? ReturnDate { get; init; }

        public SearchCriteria? SearchCriteria { get; init; }
	}
	public record DateRange
    {
        public string? Date { get; init; }
		public string? DateWindow { get; init; }
    }

	public record SearchCriteria
	{
		public int? MaxFlightOffers { get; init; }
		public int? MaxPrice { get; init; }
		public int? MaxNumberOfConnections { get; init; }
		public string[]? ExcludedCarrierCodes { get; init; }
		public string[]? IncludedCarrierCodes { get; init; }

	}
}
