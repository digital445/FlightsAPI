namespace FlightsAPI.Models
{
    public record FlightOffersQuery(
        string CurrencyCode,
        OriginDestination OriginDestination,
        int PassengerAmount);

    public record OriginDestination(
        int Id,
        string OriginLocationCode,
        string DestinationLocationCode,
        DateRange DepartureDateTimeRange,
        DateRange ArrivalDateTimeRange
    );

    public record DateRange(
        string Date, //ISO 8601 format (YYYY-MM-DD)
        string DateWindow
    );
}
