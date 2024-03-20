namespace FlightsAPI.Models
{
    public record FlightQuery(
        string CurrencyCode,
        OriginDestination OriginDestination,
        int PassengerAmount);

    public record OriginDestination(
        string OriginLocationCode,
        string DestinationLocationCode,
        DateTimeRange DepartureDateTimeRange,
        DateTimeRange ArrivalDateTimeRange
    );

    public record DateTimeRange
    {
        public string Date = DateTime.Now.ToString("yyyy-MM-dd");
        public string? DateWindow;
	}
}
