namespace FlightsAPI
{
	public static class Enumerations
	{
		public enum FlightProvider
		{
			Amadeus,
			DemoDB
		}
		public enum SortBy
		{
			None,
			/// <summary>
			/// Sort by total price of the offer
			/// </summary>
			Price,
			/// <summary>
			/// Sort by the DateTime of the outbound departure
			/// </summary>
			OutboundDeparture,
			/// <summary>
			/// Sort by the DateTime of the inbound departure
			/// </summary>
			InboundDeparture,
			/// <summary>
			/// Sort by total number of connections
			/// </summary>
			ConnectionNumber
		}
		public enum SortOrder
		{
			Ascending,
			Descending
		}

	}
}
