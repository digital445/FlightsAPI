namespace FlightsAPI.Models
{
	public record BookingOrder
	{
		public FlightOffer? FlightOffer { get; init; }
		public TravelerInfo? Traveler { get; init; }
	}

	public record AssosiatedRecord
	{
		public string? Reference { get; init; }
		public string? CreationDate { get; init; }
		public string? FlightOfferId { get; init; }
	}

	public record TravelerInfo
	{
		public string? PassengerId { get; init; }
		public string? DateOfBirth { get; init; }
		public TravelerName? Name { get; init; }
		public string? Gender { get; init; }
		public ContactInfo? Contact { get; init; }
	}
	public record TravelerName
	{
		public string? FirstName { get; init; }
		public string? LastName { get; init; }
		public string? FullName =>
			FirstName != null || LastName != null ? string.Join(' ', FirstName, LastName) : null;
    }

	public record ContactInfo
	{
		public string? EmailAddress { get; init; }
		public PhoneInfo? Phone { get; init; }
	}
	public record PhoneInfo
	{
		public string? DeviceType { get; init; }
		public string? CountryCallingCode { get; init; }
		public string? Number { get; init; }
		public string? PhoneString =>
			CountryCallingCode != null && Number != null ? $"+{CountryCallingCode}{Number}" : null;
	}
}
