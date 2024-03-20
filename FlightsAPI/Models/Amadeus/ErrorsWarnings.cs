namespace FlightsAPI.Models.Amadeus
{
	public record Error
	{
		public Issue[]? Errors { get; init; }
	}

	public record Warning
	{
		public Issue[]? Warnings { get; init; }
	}

	public record Issue
	{
		public long Code { get; init; } = -1;
		public string? Title { get; init; }
		public string? Detail { get; init; }
		public IssueSource? Source { get; init; }
	}

	public record IssueSource
	{
		public string? Pointer { get; init; }
		public string? Parameter { get; init; }
		public string? Example { get; init; }
	}
}
