namespace FlightsAPI.Models.Amadeus
{
    public record AmadeusFlightResult
    {
        public Warning[]? Warnings { get; init; }
        public AmadeusFlightOffer[]? Data { get; init; }

        public Dictionaries? Dictionaries { get; init; }
    }

	public record Dictionaries
	{
		public Dictionary<string, LocationValue>? Locations { get; init; }
		public Dictionary<string, string>? Aircrafts { get; init; }
		public Dictionary<string, string>? Currencies { get; init; }
		public Dictionary<string, string>? Carriers { get; init; }
	}

	public record LocationValue
	{
		public string? CityCode { get; init; }
		public string? CountryCode { get; init; }
	}
}
