namespace FlightsAPI.Models
{
	public record ApiRequest(
		HttpMethod Method,
		string Url,
		object Data,
		string AccessToken
	);
}
	