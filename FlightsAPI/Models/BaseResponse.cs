namespace FlightsAPI.Models
{
	public record BaseResponse(
		bool IsSuccess,
		object? Result,
		List<string> ErrorMessages
	);
}
