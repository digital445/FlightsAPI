namespace FlightsAPI.Models
{
	public record BookingQuery
	{
		public FlightOffer? FlightOffer { get; init; }
		public TravelerInfo? Traveler { get; init; }

    }

	public record TravelerInfo
	{
		public string? DateOfBirth { get; init; }
		public TravelerName? Name { get; init; }
		public string? Gender { get; init; }
		public ContactInfo? Contact { get; init; }
	}
	public record TravelerName
	{
		public string? FirstName { get; init; }
		public string? LastName { get; init; }
	}

	public record ContactInfo
	{
		public string? EmailAddress { get; init; }
		public string? PhoneType { get; init; }
		public string? CountryCallingCode { get; init; }
		public string? Number { get; init; }
	}
}
