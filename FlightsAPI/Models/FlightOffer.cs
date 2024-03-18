using static FlightsAPI.Enumerations;

namespace FlightsAPI.Models
{
    public record FlightOffer(
        int Id,
        FlightProvider FlightProvider,
        bool OneWay,
        List<Itinerary> Itineraries,
        Price Price);

    /// <summary>
    /// Avia Route
    /// </summary>
    public record Itinerary(string Duration, List<Segment> Segments);

    /// <summary>
    /// Point-to-point flight
    /// </summary>
    public record Segment(
        int Id,
        Departure Departure,
        Arrival Arrival,
        string FlightNumber, //carrierCode and number
        string AircraftCode,
        string Duration); //ISO 8601 format

    public record Departure(string IataCode, DateTime At);
    public record Arrival(string IataCode, DateTime At);
    public record Price(string Currency, decimal Total);
}
