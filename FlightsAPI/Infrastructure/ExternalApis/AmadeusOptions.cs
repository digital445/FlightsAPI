namespace FlightsAPI.Infrastructure.ExternalApis
{
	public record AmadeusOptions
	{
		public string ClientId { get; init; } = "";
		public string ClientSecret { get; init; } = "";
		public string SearchEndpoint { get; init; } = "";
		public string BookingEndpoint { get; init; } = "";
		public string TokenEndpoint { get; init; } = "";
	}
}
	