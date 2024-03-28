using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
	public class TokenService(
		ISimpleClient<HttpResponseMessage> client,
		IOptions<AmadeusOptions> Options,
		ILogger<TokenService> Logger
	) : ITokenService
	{

		private string _accessToken = "";
		private DateTime _expirationTime;
		private readonly string _clientId = Options.Value.ClientId;
		private readonly string _clientSecret = Options.Value.ClientSecret;
		private readonly string _tokenEndpoint = Options.Value.TokenEndpoint;
		private readonly JsonSerializerOptions _jOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower}; //we use specific options in this class only

		public async Task<string> GetAccessToken(bool forceRefresh = false)
		{
			if (string.IsNullOrEmpty(_accessToken) || IsTokenExpired() || forceRefresh)
			{
				await RefreshAccessToken();
			}
			return _accessToken;
		}

		//Doesn't actually work because of the scoped lifetime
		//TODO: Make the solution with Redis caching
		private bool IsTokenExpired() 
		{
			return DateTime.UtcNow >= _expirationTime;
		}

		private async Task RefreshAccessToken()
		{
			KeyValuePair<string, string>[] requestBody = {
				new("grant_type", "client_credentials"),
				new("client_id", _clientId),
				new("client_secret", _clientSecret)
			};

			ApiRequest request = new()
			{
				Method = HttpMethod.Post,
				Url = _tokenEndpoint,
				Data = requestBody
			};

			HttpResponseMessage responseMessage = await client.SendAsync(request);
			var content = await responseMessage.Content.ReadAsStringAsync();

			if (responseMessage.IsSuccessStatusCode)
			{
				var authResponse = JsonSerializer.Deserialize<AuthorizationResponse>(content, _jOptions);
				if (authResponse != null)
				{
					_accessToken = authResponse.AccessToken ?? "";
					_expirationTime = DateTime.Now.AddSeconds(authResponse.ExpiresIn);
					Logger.LogInformation("Access token is successfully refreshed.");
					return;
				}
			}
			else
			{
				var error = JsonSerializer.Deserialize<AuthorizationError>(content, _jOptions);
				if (error != null)
				{
					Logger.LogError("[{code}]: {title}. {error} -- {desc}", error.Code, error.Title, error.Error, error.ErrorDescription);
					return;
				}
			}
			Logger.LogError("Cannot get the access token.");
		}
	}
}
