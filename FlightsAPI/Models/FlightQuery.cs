namespace FlightsAPI.Models
{
	public record FlightQuery
	{
		public string CurrencyCode { get; init; } = "EUR";


        public string? OriginLocationCode { get; init; }
        public string? DestinationLocationCode { get; init; }

        public DateRange DepartureDate { get; init; } = new DateRange();
        public DateRange ReturnDate { get; init; } = new DateRange();

        public int PassengerAmount { get; init; } = 1;
	}
	public record DateRange
    {
        public string Date { get; init; }
		public string? DateWindow { get; init; }
        public DateRange()
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
		}
    }
}
