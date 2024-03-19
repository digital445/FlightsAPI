using System.Net.Http.Headers;

namespace FlightsAPI.Models
{
	public record ApiRequest(
		HttpMethod Method,
		Dictionary<string, string> AdditionalHeaders,
		string Url,
		object Data,
		string AccessToken
	);
}
	