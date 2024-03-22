namespace FlightsAPI.Infrastructure.ExternalApis
{
	public record AmadeusOptions
	{
		public string? AccessToken { get; init; }
		public string? ClientId { get; init; }
		public string? ClientSecret { get; init; }
		public string? SearchBaseUrl { get; init; }
		public string? BookingBaseUrl { get; init; }
	}
}
