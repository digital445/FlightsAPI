using FlightsAPI.Models;

namespace FlightsAPI.Infrastructure.ExternalApis.Interfaces
{
    public interface ISimpleClient<T> where T : class
    {
		Task<T> SendAsync(ApiRequest apiRequest);
	}
}
