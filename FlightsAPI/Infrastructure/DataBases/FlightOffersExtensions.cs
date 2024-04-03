using FlightsAPI.Models;

namespace FlightsAPI.Infrastructure.DataBases
{
	public static class FlightOffersExtensions
	{
		/// <summary>
		/// Set Id to each flight offer in flight offer collection
		/// </summary>
		/// <param name="flightOffers"></param>
		public static IEnumerable<FlightOffer> SetFlightOfferIds(this IEnumerable<FlightOffer> flightOffers)
		{
			int i = 1;
			foreach (var fl in flightOffers)
				fl.Id = i++.ToString();
			return flightOffers;
		}
	}
}
