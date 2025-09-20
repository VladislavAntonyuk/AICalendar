using System.Security.Claims;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.Events.Create;

internal static class CreateEvent
{
	public static RouteGroupBuilder MapCreateEvent(this RouteGroupBuilder routes)
	{
		routes.MapPost("/", Handler)
		      .WithName("Create Event")
		      .WithSummary("Create Event");

		return routes;
	}

	private static async Task<IResult> Handler(
		Request? request,
		ClaimsPrincipal claims,
		CreateEventHandler handler,
		CancellationToken cancellationToken)
	{
		if (request is null)
		{
			return ApiResults.Problem(Result.Failure(Error.NullValue));
		}

		var result = await handler.Handle(claims.GetUserId(), request, cancellationToken);
		return result.Match(Results.Ok, ApiResults.Problem);
	}

	internal sealed class Request
	{
		public DateTime Start { get; init; }
		public DateTime End { get; init; }
		public string? Title { get; set; }
		public ICollection<string> Attendees { get; set; } = new List<string>();
	}

	internal record ResponseContent(Guid Id);
}