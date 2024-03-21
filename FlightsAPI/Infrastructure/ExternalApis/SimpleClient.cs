using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	/// <summary>
	/// Simple http-client which sends ApiRequest and receives HttpResponseMessage
	/// </summary>
	public class SimpleClient(
		IHttpClientFactory HttpClientFactory, 
		IOptions<JsonSerializerOptions> JOptions) : ISimpleClient<HttpResponseMessage>
	{
		public async Task<HttpResponseMessage> SendAsync(ApiRequest apiRequest)
		{
			using var client = HttpClientFactory.CreateClient("ExternalAPI");

			InjectAccessToken(apiRequest.AccessToken, client);
			HttpRequestMessage message = CreateMessage(apiRequest);

			HttpResponseMessage httpResponse = await client.SendAsync(message);

			return httpResponse;
		}

		/// <summary>
		/// Inject the external service access token into client's default headers.
		/// </summary>
		private static void InjectAccessToken(string? accessToken, HttpClient client)
		{
			client.DefaultRequestHeaders.Clear();
			if (!string.IsNullOrEmpty(accessToken))
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			}
		}

		private HttpRequestMessage CreateMessage(ApiRequest apiRequest)
		{
			string json = JsonSerializer.Serialize(apiRequest.Data, JOptions.Value);
			HttpRequestMessage message = new()
			{
				Method = apiRequest.Method,
				RequestUri = new Uri(apiRequest.Url),
				Content = apiRequest.Data != null
					? new StringContent(json, Encoding.UTF8, "application/json")
					: null
			};

			if (apiRequest.AdditionalHeaders != null)
			{
                foreach (var header in apiRequest.AdditionalHeaders) 
				{
					message.Headers.Add(header.Key, header.Value);
				}
			}

			return message;
		}
	}
}
