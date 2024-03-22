using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
    public class AmadeusClient(
        ISimpleClient<HttpResponseMessage> Client, 
        IOptions<AmadeusOptions> Options,
        IOptions<JsonSerializerOptions> JOptions,
        ILogger<AmadeusClient> Logger) : IAmadeusClient
	{
        private readonly string? _accessToken = Options.Value.AccessToken;
        private readonly string _searchBaseUrl = Options.Value.SearchBaseUrl ?? "";
        private readonly string _bookingBaseUrl = Options.Value.BookingBaseUrl ?? "";

		public async Task<IEnumerable<AmadeusFlightOffer>> GetFlightOffers(AmadeusFlightQuery query)
        {
            try
            {
                Dictionary<string, string> additionalHeaders = new() {{ "X-HTTP-Method-Override", "GET"}};
                ApiRequest request = new(
                    Method: HttpMethod.Post,
                    AdditionalHeaders : additionalHeaders, 
                    Url: _searchBaseUrl + "/flight-offers",
                    Data: query,
                    AccessToken: _accessToken);

                HttpResponseMessage httpResponse = await Client.SendAsync(request);
			    string apiContent = await httpResponse.Content.ReadAsStringAsync();

                return ExtractFlightOffers((int)httpResponse.StatusCode, apiContent);
            }
            catch (Exception ex)
            {
				Logger.LogError(ex, "Cannot get flight offers.");
				return [];
			}
		}
        
		public Task<AmadeusBookingResult> BookFlights(AmadeusBookingQuery query)
		{
			throw new NotImplementedException();
		}

        private AmadeusFlightOffer[] ExtractFlightOffers(int statusCode, string apiContent)
        {
            if (statusCode == 200)
            {
				var amadeusResponse = JsonSerializer.Deserialize<AmadeusResponse>(apiContent, JOptions.Value);
                if (amadeusResponse?.Data != null)
                {
				    Logger.LogInformation("Got response from Amadeus API.");
                    return amadeusResponse.Data;
                }
            }
			Logger.LogError("Status {Status}: cannot get flight offers from Amadeus", statusCode);

			if (statusCode >= 400)
            {
				var errorResponse = JsonSerializer.Deserialize<Error>(apiContent, JOptions.Value);
				errorResponse?.Errors?
                    .ToList()
                    .ForEach(er => Logger.LogError("[{Code}] {Title}: {Detail}", er.Code, er.Title, er.Detail));
			}
            return [];
        }
	}
}
