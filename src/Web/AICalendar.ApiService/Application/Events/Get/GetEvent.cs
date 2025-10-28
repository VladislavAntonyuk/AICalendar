using System.Security.Claims;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events.Get;

internal static class GetEvent
{
	public static RouteGroupBuilder MapGetEvent(this RouteGroupBuilder routes)
	{
		routes.MapGet("/{id:guid}", Handler).WithName("Get Event").WithSummary("Get Event");

		return routes;
	}

	private static async Task<IResult> Handler(Guid id,
		ClaimsPrincipal claims,
		GetEventHandler handler,
		CancellationToken cancellationToken)
	{
		var result = await handler.Handle(claims.GetUserId(), id, cancellationToken);
		return result.Match(Results.Ok, ApiResults.Problem);
	}
}