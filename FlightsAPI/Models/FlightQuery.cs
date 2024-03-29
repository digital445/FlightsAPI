using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.Models
{
	public record FlightQuery
	{
		public string? CurrencyCode { get; init; }


        public string? OriginLocationCode { get; init; }
        public string? DestinationLocationCode { get; init; }

        public DateRange? DepartureDate { get; init; }
        public DateRange? ReturnDate { get; init; }

        public SearchCriteria? SearchCriteria { get; init; }
        public SortCriteria SortCriteria { get; init; } = new() { Criteria = SortBy.None };
	}
	public record DateRange
    {
        public string? Date { get; init; }
		public string? DateWindow { get; init; }
    }

	public record SearchCriteria
	{
		public int? MaxFlightOffers { get; init; }
		public int? MaxPrice { get; init; }
		public int? MaxNumberOfConnections { get; init; }
		public string[]? ExcludedCarrierCodes { get; init; }
		public string[]? IncludedCarrierCodes { get; init; }
	}
	public record SortCriteria
	{
        public SortBy Criteria { get; init; }
        public SortOrder Order { get; init; }

    }
}
