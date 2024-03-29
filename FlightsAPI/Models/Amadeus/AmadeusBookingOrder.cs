namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusBookingQuery
	{
		public AmadeusBookingOrder? Data { get; init; }
	}
	public record AmadeusBookingOrder
	{
		public string? Id { get; init; }
		public string Type { get; } = "flight-order";
        public AmAssosiatedRecord[]? AssociatedRecords { get; init; }
        public AmadeusFlightOffer[]? FlightOffers { get; init; }
		public AmTravelerInfo[]? Travelers { get; init; }
	}

	public record AmAssosiatedRecord
	{
        public string? Reference { get; init; }
		public string? CreationDate { get; init; }
		public string? FlightOfferId { get; init; }
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
