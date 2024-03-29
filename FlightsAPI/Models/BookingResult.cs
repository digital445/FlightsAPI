namespace FlightsAPI.Models
{
	public record BookingResult
	{
		public string? Id { get; init; }
		public AssosiatedRecord[]? AssociatedRecords { get; init; }

        public IEnumerable<OrderIssue>? Issues { get; init; }
    }

	public record OrderIssue
	{
		public string? Title { get; init; }
        public string? Detail { get; init; }
    }
}
