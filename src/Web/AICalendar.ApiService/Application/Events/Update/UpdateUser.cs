using AICalendar.ApiService.Application.User.Create;
using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.User.Update;

internal static class UpdateUser
{
	public static RouteGroupBuilder MapUpdateUsers(this RouteGroupBuilder routes)
	{
		routes.MapPost("/users/{id:guid}", Handler)
			  .WithName("Update Users")
			  .WithSummary("Update Users");

		return routes;
	}

	private static async Task<IResult> Handler(
		Request? request,
		UpdateUsersHandler handler,
		CancellationToken cancellationToken)
	{
		if (request is null)
		{
			return ApiResults.Problem(Result.Failure(Error.NullValue));
		}

		var result = await handler.Handle(request, cancellationToken);
		return result.Match(Results.Ok, ApiResults.Problem);
	}

	internal sealed class Request
	{
		public required Guid Id { get; init; }

		public required string Prop1 { get; init; }
	}

	internal record ResponseContent(Guid Id);
}