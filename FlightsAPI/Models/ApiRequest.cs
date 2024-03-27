namespace FlightsAPI.Models
{
	public record ApiRequest
	{
		public HttpMethod Method { get; init; } = HttpMethod.Get;
		public Dictionary<string, string>? AdditionalHeaders { get; init; }
		public string Url { get; init; } = "";
		public object? Data { get; init; }
		public string AccessToken { get; init; } = "";
	}
}