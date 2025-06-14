using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events.Get;

internal static class GetEvent
{
	public static RouteGroupBuilder MapGetEvent(this RouteGroupBuilder routes)
	{
		routes.MapGet("/{id:guid}", Handler)
		      .WithName("Get Event")
		      .WithSummary("Get Event");

		return routes;
	}

	private static async Task<IResult> Handler(
		Guid id,
		GetEventHandler handler,
		CancellationToken cancellationToken)
	{
		Guid.TryParse("8408511A-CBBE-4770-8145-ED01EF1741BC", out var currentUserId);
		var result = await handler.Handle(currentUserId, id, cancellationToken);
		return result.Match(Results.Ok, ApiResults.Problem);
	}

	internal record ResponseContent(
		Guid Id,
		DateTime Start,
		DateTime End,
		string? Title,
		Guid OrganizerId,
		ICollection<string> Attendees);
}