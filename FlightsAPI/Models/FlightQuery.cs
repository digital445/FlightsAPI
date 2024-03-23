﻿namespace FlightsAPI.Models
{
	public record FlightQuery
	{
		public string CurrencyCode { get; init; } = "EUR";


        public string? OriginLocationCode { get; init; }
        public string? DestinationLocationCode { get; init; }

        public DateRange DepartureDate { get; init; } = new DateRange();
        public DateRange ReturnDate { get; init; } = new DateRange();

        public SearchCriteria? SearchCriteria { get; init; }

        public int PassengerAmount { get; init; } = 1;
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
}
