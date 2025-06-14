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
		DeleteEventHandler handler,
		CancellationToken cancellationToken)
	{
		Guid.TryParse("8408511A-CBBE-4770-8145-ED01EF1741BC", out var currentUserId);
		var result = await handler.Handle(id, currentUserId, cancellationToken);
		return result.Match(Results.NoContent, ApiResults.Problem);
	}
}