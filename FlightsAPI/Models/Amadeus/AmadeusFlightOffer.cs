namespace FlightsAPI.Models.Amadeus
{
	public record AmadeusFlightOffer(
		string Type,
		string Id,
		string Source,
		bool DisablePricing,
		bool OneWay,
		int NumberOfBookableSeats,
		Itinerary[] Itineraries
	);
	/// <summary>
	/// Avia Route
	/// </summary>
	public record Itinerary(string Duration, Segment[] Segments);

	/// <summary>
	/// Point-to-point flight
	/// </summary>
	public record Segment(
		string Id,
		Departure Departure,
		Arrival Arrival,
		string CarrierCode,
		string Number,
		Aircraft Aircraft,
		string Duration); //ISO 8601 format (PnYnMnDTnHnMnS)

	public record Departure(string IataCode, DateTime At);
	public record Arrival(string IataCode, DateTime At);
	public record Aircraft(string Code);

}
