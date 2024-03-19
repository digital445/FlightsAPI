namespace FlightsAPI.Models.Amadeus
{
    public record AmadeusFlightOfferQuery(
        string CurrencyCode,
        OriginDestination[] OriginDestinations,
        ExtendedTravelInfo[] Travelers,
        SearchCriteria SearchCriteria
    )
    {
        public string[] Sources { get; } = ["GDS"];
    };

    public record OriginDestination(
        string Id,
        string OriginLocationCode,
        string DestinationLocationCode,
        DateTimeRange DepartureDateTimeRange,
        DateTimeRange ArrivalDateTimeRange
    );

    public record ExtendedTravelInfo(string Id, string TravelerType);

    public record DateTimeRange(
        string Date,
        string DateWindow,
        string Time,
        string TimeWindow
    );

    public record SearchCriteria(
        bool AddOnewayOffers,
        int MaxPrice,
        FlightFilters FlightFilters
    );

    public record FlightFilters(CarrierRestrictions CarrierRestrictions, ConnectionRestriction ConnectionRestriction);
    public record CarrierRestrictions(string[] ExcludedCarrierCodes, string[] IncludedCarrierCodes);
    public record ConnectionRestriction(int MaxNumberOfConnections);
}