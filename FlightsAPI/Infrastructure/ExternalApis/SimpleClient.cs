using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="httpClientFactory"></param>
	/// <param name="logger"></param>
	public class SimpleClient(IHttpClientFactory httpClientFactory, ILogger<SimpleClient> logger) : ISimpleClient<HttpResponseMessage>
	{
		public async Task<HttpResponseMessage> SendAsync(ApiRequest apiRequest)
		{
			try
			{
				using var client = httpClientFactory.CreateClient("ExternalAPI");

				InjectAccessToken(apiRequest, client);
				HttpRequestMessage message = CreateMessage(apiRequest);

				HttpResponseMessage httpResponse = await client.SendAsync(message);

				return httpResponse;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Exception occured while getting the response from the external API.");
				return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
			}
		}

		/// <summary>
		/// Inject the external service access token into client's default headers.
		/// </summary>
		private static void InjectAccessToken(ApiRequest apiRequest, HttpClient client)
		{
			client.DefaultRequestHeaders.Clear();
			if (!string.IsNullOrEmpty(apiRequest.AccessToken))
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
			}
		}

		private static HttpRequestMessage CreateMessage(ApiRequest apiRequest)
		{
			HttpRequestMessage message = new()
			{
				Method = apiRequest.Method,
				RequestUri = new Uri(apiRequest.Url),
				Content = apiRequest.Data != null
					? new StringContent(
						JsonSerializer.Serialize(apiRequest.Data),
						Encoding.UTF8,
						"application/json"
					  )
					: null
			};

			foreach (var header in apiRequest.AdditionalHeaders) 
			{
				message.Headers.Add(header.Key, header.Value);
			}

			return message;
		}
	}
}
