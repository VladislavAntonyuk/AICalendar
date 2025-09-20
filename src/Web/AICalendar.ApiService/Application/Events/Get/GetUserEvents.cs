using System.Security.Claims;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events.Get;

internal static class GetUserEvents
{
	public static RouteGroupBuilder MapCurrentUserEvents(this RouteGroupBuilder routes)
	{
		routes.MapGet("/", GetCurrentUserEventsHandler)
		      .WithName("Get Events")
		      .WithSummary("Get Current User Events");

		return routes;
	}

	private static async Task<IResult> GetCurrentUserEventsHandler(
		[AsParameters] GetEventsRange range,
		ClaimsPrincipal claims,
		GetUserEventsHandler handler,
		CancellationToken cancellationToken)
	{
		var result = await handler.Handle(claims.GetUserId(), range, cancellationToken);
		return result.Match(Results.Ok, Results.NotFound);
	}

	internal record GetEventsRange(DateTime From, DateTime To);
}