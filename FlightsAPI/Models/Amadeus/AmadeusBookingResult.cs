namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusBookingResult
	{
        public Issue[]? Warnings { get; init; }
        public AmadeusBookingOrder? Data { get; init; }
    }
}
