using AutoMapper;
using FlightsAPI.MapperProfiles;
using FlightsAPI.Models;
using FlightsAPI.Models.Amadeus;
using System.Globalization;
using System.Text.Json;

namespace MappingAndSerialization
{
	public class MappingAndSerialization
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
				OriginLocationCode = "NYC",
				DestinationLocationCode = "MAD",
				DepartureDate = new DateRange { Date = DateTime.ParseExact("2024-04-15", "yyyy-MM-dd", CultureInfo.InvariantCulture)},
				ReturnDate = new DateRange { Date = DateTime.ParseExact("2024-04-16", "yyyy-MM-dd", CultureInfo.InvariantCulture) }
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
			string queryJson = "{\"originLocationCode\":\"NYC\",\"destinationLocationCode\":\"MAD\",\"departureDate\":{\"date\":\"2024-04-15\"},\"returnDate\":{\"date\":\"2024-04-16\"}}";
			var actual = JsonSerializer.Deserialize<FlightQuery>(queryJson, _jOptions);

			var expected = new FlightQuery()
			{
				OriginLocationCode = "NYC",
				DestinationLocationCode = "MAD",
				DepartureDate = new DateRange { Date = DateTime.ParseExact("2024-04-15", "yyyy-MM-dd", CultureInfo.InvariantCulture) },
				ReturnDate = new DateRange { Date = DateTime.ParseExact("2024-04-16", "yyyy-MM-dd", CultureInfo.InvariantCulture) }
			};

			Assert.Equal(expected, actual);

		}
	}
}