namespace FlightsAPI.Models.Amadeus
{
    public record AmadeusResponse(
        Warning[] Warnings,
        FlightOffer[] Data
    );
}
