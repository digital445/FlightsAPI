namespace FlightsAPI.Models.Amadeus
{	public record AuthorizationResponse
	{
		public string? Type { get; init; }
		public string? Username { get; init; }
		public string? ApplicationName { get; init; }
		public string? ClientId { get; init; }
		public string? TokenType { get; init; }
		public string? AccessToken { get; init; }
		public int ExpiresIn { get; init; }
		public string? State { get; init; }
		public string? Scope { get; init; }
	}
}
