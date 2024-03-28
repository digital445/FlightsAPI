using AutoMapper;
using FlightsAPI.MapperProfiles;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using System.Text.Json;

namespace MappingAndSerialization
{
	public class MappingTests
	{
		private IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new FlightQueryProfile())).CreateMapper();
		private JsonSerializerOptions _jOptions = new()
		{
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true
		};

		[Fact]
		public void MapQueryToAmadeusQuery()
		{
			FlightQuery flightQuery = new()
			{
				CurrencyCode = "USD",
				OriginLocationCode = "NYC",
				DestinationLocationCode = "MAD",
				DepartureDate = new DateRange { Date = "2024-04-15" },
				ReturnDate = new DateRange { Date = "2024-04-16" }
			};

			var actual = _mapper.Map<AmadeusFlightQuery>(flightQuery);

			AmadeusFlightQuery expected = new()
			{
				CurrencyCode = "USD",
				OriginDestinations = [
					new AmOriginDestination
					{
						Id = "1",
						OriginLocationCode = "NYC",
						DestinationLocationCode = "MAD",
						DepartureDateTimeRange = new AmDateTimeRange { Date = "2024-04-15" }
					},
					new AmOriginDestination
					{
						Id = "2",
						OriginLocationCode = "MAD",
						DestinationLocationCode = "NYC",
						DepartureDateTimeRange = new AmDateTimeRange { Date = "2024-04-16" }
					}
				],
				Travelers = [
					new AmShortTravelerInfo { Id = "1", TravelerType = "ADULT" }
				],
				Sources = ["GDS"]
			};


			string actualJson = JsonSerializer.Serialize(actual, _jOptions);
			string expectedJson = JsonSerializer.Serialize(expected, _jOptions);

			Assert.Equal(expectedJson, actualJson);
		}

		[Fact]
		public void FlightQueryDeserialization()
		{
			string queryJson = "{\"currencyCode\":\"USD\",\"OriginLocationCode\":\"NYC\",\"DestinationLocationCode\":\"MAD\",\"DepartureDate\":{\"Date\":\"2024-04-15\"},\"ReturnDate\":{\"Date\":\"2024-04-16\"}}";
			var actual = JsonSerializer.Deserialize<FlightQuery>(queryJson, _jOptions);

			var expected = new FlightQuery()
			{
				CurrencyCode = "USD",
				OriginLocationCode = "NYC",
				DestinationLocationCode = "MAD",
				DepartureDate = new DateRange { Date = "2024-04-15" },
				ReturnDate = new DateRange { Date = "2024-04-16" }
			};

			Assert.Equal(expected, actual);

		}
	}
}