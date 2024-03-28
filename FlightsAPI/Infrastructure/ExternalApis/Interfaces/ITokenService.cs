namespace FlightsAPI.Infrastructure.ExternalApis.Interfaces
{
	public interface ITokenService
	{
		Task<string> GetAccessToken(bool forceRefresh = false);
	}
}
