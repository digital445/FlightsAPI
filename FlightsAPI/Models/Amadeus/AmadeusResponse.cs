namespace FlightsAPI.Models.Amadeus
{
    public record AmadeusResponse
    {
        public Warning[]? Warnings { get; init; }
        public AmadeusFlightOffer[]? Data { get; init; }
    }
}
