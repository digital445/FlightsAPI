namespace FlightsAPI.Models.FlightDb
{
	public partial class Booking
	{
		public static string GetRandomBookRef() =>
			new Random().Next(16777215).ToString("X6");
	}
}
