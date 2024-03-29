using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FlightsAPI.Infrastructure.ExternalApis
{
    public class AmadeusClient(
        ISimpleClient<HttpResponseMessage> Client,
        ITokenService TokenService,
        IOptions<AmadeusOptions> Options,
        IOptions<JsonSerializerOptions> JOptions,
        ILogger<AmadeusClient> Logger) : IAmadeusClient
	{
        private readonly string _searchEndpoint = Options.Value.SearchEndpoint ?? "";
        private readonly string _bookingEndpoint = Options.Value.BookingEndpoint ?? "";

		public async Task<IEnumerable<AmadeusFlightOffer>> GetFlightOffers(AmadeusFlightQuery query)
        {
            try
            {
                Dictionary<string, string> additionalHeaders = new() {{ "X-HTTP-Method-Override", "GET"}};
                ApiRequest request = new()
                {
                    Method = HttpMethod.Post,
                    AdditionalHeaders = additionalHeaders,
                    Url = _searchEndpoint,
                    Data = query,
                    AccessToken = await TokenService.GetAccessToken()
                };

                HttpResponseMessage httpResponse = await Client.SendAsync(request);

                return await ExtractFlightOffers(httpResponse);
            }
            catch (Exception ex)
            {
				Logger.LogError(ex, "Cannot get flight offers.");
				return [];
			}
		}
        
		public async Task<AmadeusBookingOrder> BookFlights(AmadeusBookingQuery query)
		{
			try
			{
                ApiRequest request = new()
                {
                    Method = HttpMethod.Post,
                    AdditionalHeaders = null,
                    Url = _bookingEndpoint,
                    Data = query,
                    AccessToken = await TokenService.GetAccessToken()
                };

				HttpResponseMessage httpResponse = await Client.SendAsync(request);

				return await ExtractBookingOrder(httpResponse);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Cannot book the flight.");
				return new AmadeusBookingOrder();
			}
		}

		private async Task<AmadeusBookingOrder> ExtractBookingOrder(HttpResponseMessage httpResponse)
		{
			string apiContent = await httpResponse.Content.ReadAsStringAsync();
			if (httpResponse.IsSuccessStatusCode)
			{
				var bookingResult = JsonSerializer.Deserialize<AmadeusBookingResult>(apiContent, JOptions.Value);
				if (bookingResult?.Warnings != null)
				{
					bookingResult.Warnings?
						.ToList()
						.ForEach(wrn => Logger.LogWarning("[{Code}] {Title}: {Detail}", wrn.Code, wrn.Title, wrn.Detail));
				}
				if (bookingResult?.Data != null)
				{
					Logger.LogInformation("Got booking result from Amadeus API.");
					return bookingResult.Data;
				}
			}
			else
			{
				var errorResult = JsonSerializer.Deserialize<Error>(apiContent, JOptions.Value);
				errorResult?.Errors?
					.ToList()
					.ForEach(er => Logger.LogError("[{Code}] {Title}: {Detail}", er.Code, er.Title, er.Detail));
			}
			return new AmadeusBookingOrder();
		}

		private async Task<AmadeusFlightOffer[]> ExtractFlightOffers(HttpResponseMessage httpResponse)
        {
			string apiContent = await httpResponse.Content.ReadAsStringAsync();
			if (httpResponse.IsSuccessStatusCode)
            {
				var flightResult = JsonSerializer.Deserialize<AmadeusFlightResult>(apiContent, JOptions.Value);
                if (flightResult?.Data != null)
                {
				    Logger.LogInformation("Got flight offers from Amadeus API.");
                    return flightResult.Data;
                }
			    Logger.LogError("The Amadeus response is empty.");
            }
            else
            {
			    var errorResult = JsonSerializer.Deserialize<Error>(apiContent, JOptions.Value);
			    errorResult?.Errors?
                    .ToList()
                    .ForEach(er => Logger.LogError("[{Code}] {Title}: {Detail}", er.Code, er.Title, er.Detail));
            }
            return [];
        }
	}
}
