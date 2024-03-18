using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	public class BaseClient(IHttpClientFactory httpClientFactory, ILogger<BaseClient> logger) : IBaseClient
	{
		public async Task<T?> SendAsync<T>(ApiRequest apiRequest) where T : BaseResponse
		{
			try
			{
				using var client = httpClientFactory.CreateClient("ExternalAPI");

				PrepareHeaders(apiRequest, client);
				HttpRequestMessage message = CreateMessage(apiRequest);

				HttpResponseMessage httpResponse = await client.SendAsync(message);

				string apiContent = await httpResponse.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(apiContent))
				{
					throw new Exception("HTTP-response: " + (int)httpResponse.StatusCode + " -- " + httpResponse.ReasonPhrase);
				}

				var response = JsonSerializer.Deserialize<T>(apiContent);
				logger.LogInformation("Got response from the external API.");

				return response;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Exception occured while getting the response from the external API.");

				var baseResponse = new BaseResponse(false, null, [ex.Message]);
				var json = JsonSerializer.Serialize(baseResponse);
				var response = JsonSerializer.Deserialize<T>(json); //use serialization/deserialization for returning an object of T type
				return response;
			}
		}

		private static void PrepareHeaders(ApiRequest apiRequest, HttpClient client)
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
			message.Headers.Add("Accept", "application/json");

			return message;
		}
	}
}
