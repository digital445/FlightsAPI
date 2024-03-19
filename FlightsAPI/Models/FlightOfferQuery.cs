namespace FlightsAPI.Models
{
    public record FlightOfferQuery(
        string CurrencyCode,
        OriginDestination OriginDestination,
        int PassengerAmount);

    public record OriginDestination(
        int Id,
        string OriginLocationCode,
        string DestinationLocationCode,
        DateTimeRange DepartureDateTimeRange,
        DateTimeRange ArrivalDateTimeRange
    );

    public record DateTimeRange(
        string Date, //ISO 8601 format (YYYY-MM-DD)
        string DateWindow
    );
}
