using FlightsAPI.Models;

namespace FlightsAPI.Infrastructure.ExternalApis.Interfaces
{
    public interface IBaseClient
    {
		Task<T?> SendAsync<T>(ApiRequest apiRequest) where T : BaseResponse;
	}
}
