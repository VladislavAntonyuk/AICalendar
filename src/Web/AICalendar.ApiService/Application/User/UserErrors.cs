using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.User;

public static class UserErrors
{
	public static Error NotFound(Guid userId)
	{
		return Error.NotFound("Users.NotFound", $"The User with the identifier {userId} not found");
	}

	public static Error NotFound(string identityId)
	{
		return Error.NotFound("Users.NotFound", $"The User with the IDP identifier {identityId} not found");
	}
}