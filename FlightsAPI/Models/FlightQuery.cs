using System.Text.RegularExpressions;
using static FlightsAPI.Enumerations;

namespace FlightsAPI.Models
{
	public record FlightQuery
	{
        public string? OriginLocationCode { get; init; }
        public string? DestinationLocationCode { get; init; }

        public DateRange? DepartureDate { get; init; }
        public DateRange? ReturnDate { get; init; }

        public SearchCriteria? SearchCriteria { get; init; }
        public SortCriteria SortCriteria { get; init; } = new() { Criteria = SortBy.None };
	}
	public record DateRange
    {
        public DateTime Date { get; init; }
		public string? DateWindow { get; init; }
		public DateTime MinDate => CalculateBoundaryDate(minimal: true);
		public DateTime MaxDate => CalculateBoundaryDate(minimal: false);
		private DateTime CalculateBoundaryDate(bool minimal)
		{
			if (DateWindow == null)
				return Date;

			string pattern = @"^([MPI])([1-3])D";
			var match = Regex.Match(DateWindow, pattern);
			if (match.Success)
			{
				string mode = match.Groups[1].Value;
				int daysNum = int.Parse(match.Groups[2].Value) * (minimal ? -1 : 1);
				return mode switch
				{
					"I" => Date.AddDays(daysNum),
					"M" => minimal ? Date.AddDays(daysNum) : Date,
					"P" => minimal ? Date : Date.AddDays(daysNum),
					_ => Date
				};
			}
			throw new FormatException($"The {nameof(DateWindow)} string contains unexpected content. Expected is ^[MPI][1-3]D");
		}
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
