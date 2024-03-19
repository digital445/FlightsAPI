namespace FlightsAPI.Models.Amadeus
{
    public record Error(Issue[] Errors);
    public record Warning(Issue[] Warnings);
    public record Issue(long Code, string Title, string Detail, IssueSource Source);
    public record IssueSource(string Pointer, string Parameter, string Example);
}
