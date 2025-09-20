using AICalendar.ApiService.Application.User.Delete;
using AICalendar.ApiService.Application.User.Get;

namespace AICalendar.ApiService.Application.User;

internal static class UsersModule
{
	public static IEndpointRouteBuilder MapUserRoutes(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/v1/users")
		                  .WithTags("Users")
						  .RequireAuthorization();

		group.MapGetUser();
		group.MapCurrentUser();
		group.MapDeleteUsers();

		return group;
	}
}