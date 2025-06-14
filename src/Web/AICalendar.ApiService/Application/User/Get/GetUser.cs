using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.User.Get;

internal static class GetUser
{
	public static RouteGroupBuilder MapGetUser(this RouteGroupBuilder routes)
	{
		routes.MapGet("/{id:guid}", Handler)
		      .WithName("Get User By Id")
		      .WithSummary("Get User By Id");

		return routes;
	}
	
	public static RouteGroupBuilder MapCurrentUser(this RouteGroupBuilder routes)
	{
		routes.MapGet("/me", GetCurrentUserHandler)
		      .WithName("Get Current User")
		      .WithSummary("Get Current User")
		      .RequireAuthorization();

		return routes;
	}
	
	private static async Task<IResult> Handler(
		Guid id,
		GetUserHandler handler,
		CancellationToken cancellationToken)
	{
		var result = await handler.Handle(id, cancellationToken);
		return result.Match(Results.Ok, Results.NotFound);
	}

	private static async Task<IResult> GetCurrentUserHandler(
		HttpContext context,
		GetUserHandler handler,
		CancellationToken cancellationToken)
	{
		Guid.TryParse("8408511A-CBBE-4770-8145-ED01EF1741BC", out var id);
		var result = await handler.Handle(id, cancellationToken);
		return result.Match(Results.Ok, Results.NotFound);
	}

	internal record ResponseContent(Guid Id);
}