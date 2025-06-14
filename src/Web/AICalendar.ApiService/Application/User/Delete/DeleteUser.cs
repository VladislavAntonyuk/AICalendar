using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.User.Delete;

internal static class DeleteUser
{
	public static RouteGroupBuilder MapDeleteUsers(this RouteGroupBuilder routes)
	{
		routes.MapDelete("/{id:guid}", Handler)
		      .WithName("Delete Users")
		      .WithSummary("Delete Users");

		return routes;
	}

	private static async Task<IResult> Handler(
		Guid id,
		DeleteUsersHandler handler,
		CancellationToken cancellationToken)
	{
		var result = await handler.Handle(id, cancellationToken);
		return result.Match(Results.NoContent, ApiResults.Problem);
	}
}