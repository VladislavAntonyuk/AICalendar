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
		HttpContext context,
		[AsParameters] GetEventsRange range,
		GetUserEventsHandler handler,
		CancellationToken cancellationToken)
	{
		Guid.TryParse("8408511A-CBBE-4770-8145-ED01EF1741BC", out var id);
		var result = await handler.Handle(id, range, cancellationToken);
		return result.Match(Results.Ok, Results.NotFound);
	}

	internal record GetEventsRange(DateTime From, DateTime To);
}