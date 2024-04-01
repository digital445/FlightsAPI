namespace FlightsAPI.Infrastructure.DataBases
{
	public record FlightDbOptions
	{
        public string? ConnectionString { get; init; }
        /// <summary>
        /// Minimum Connecting Time between consecutive flights
        /// </summary>
        public int MCT { get; init; }
		/// <summary>
		/// Maximum number of return flights to combine with one outbound flight
		/// </summary>
        public int MaxReturnFlights { get; init; }
        public decimal UsdToRubConversionRate { get; init; }
    }
}
