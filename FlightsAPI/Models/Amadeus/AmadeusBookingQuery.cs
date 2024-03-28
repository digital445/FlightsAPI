namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusBookingQuery
	{
		public OrderData? Data { get; init; }
	}
	public record OrderData
	{
		public string Type { get; } = "flight-order";
		public AmadeusFlightOffer[]? FlightOffers { get; init; }
		public AmTravelerInfo[]? Travelers { get; init; }
	}
	public record AmTravelerInfo
	{
		public string? Id { get; } = "1";
		public string? DateOfBirth { get; init; }
		public AmTravelerName? Name { get; init; }
		public string? Gender { get; init; }
		public AmContactInfo? Contact { get; init; }
	}
	public record AmTravelerName
	{
		public string? FirstName { get; init; }
		public string? LastName { get; init; }
	}

	public record AmContactInfo
	{
		public string? EmailAddress { get; init; }
		public AmPhoneInfo[]? Phones { get; init; }
	}
	public record AmPhoneInfo
	{
		public string? DeviceType { get; init; }
		public string? CountryCallingCode { get; init; }
		public string? Number { get; init; }
	}
}
