namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusBookingQuery
	{
		public OrderData? Data { get; init; }
	}
	public record OrderData
	{
		public string Type { get; } = "flight-order";
		public AmadeusFlightOffer?[] FlightOffers { get; } = new AmadeusFlightOffer[1];
        public TravelerInfo[] Travelers { get; init; } = [new TravelerInfo()];
    }
	public record TravelerInfo
	{
		public string Id { get; init; } = "1";
		public string DateOfBirth { get; init; } = "1982-01-16";
		public TravelerName Name { get; init; } = new();
		public string Gender { get; init; } = "MALE";
        public ContactInfo Contact { get; init; } = new();
    }
	public record TravelerName
	{
		public string FirstName { get; init; } = "MIGUEL";
		public string LastName { get; init; } = "GONZALES";
	}

	public record ContactInfo
	{
		public string EmailAddress { get; init; } = "miguel.gonzales833@telefonica.es";
		public PhoneInfo[] Phones { get; init; } = [new PhoneInfo()];
	}
	public record PhoneInfo
	{
		public string DeviceType { get; init; } = "MOBILE";
		public string CountryCallingCode { get; init; } = "34";
		public string Number { get; init; } = "480080076";
	}
}
