using System.Security.Claims;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events.Delete;

internal static class DeleteEvent
{
	public static RouteGroupBuilder MapDeleteEvent(this RouteGroupBuilder routes)
	{
		routes.MapDelete("/{id:guid}", Handler)
		      .WithName("Cancel Event")
		      .WithSummary("Cancel Event");

		return routes;
	}

	private static async Task<IResult> Handler(
		Guid id,
		ClaimsPrincipal claims,
		DeleteEventHandler handler,
		CancellationToken cancellationToken)
	{
		var result = await handler.Handle(id, claims.GetUserId(), cancellationToken);
		return result.Match(Results.NoContent, ApiResults.Problem);
	}
}