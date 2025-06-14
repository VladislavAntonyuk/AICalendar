using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.Extensions.AI;

namespace AICalendar.ApiService.Application.AI;

internal static class AiEvent
{
	public static RouteGroupBuilder MapAi(this RouteGroupBuilder routes)
	{
		routes.MapPost("/", Handler)
		      .WithName("AI")
		      .WithSummary("AI");

		return routes;
	}

	private static IAsyncEnumerable<ChatResponseUpdate> Handler(
		AiHandler handler,
		Request request,
		CancellationToken cancellationToken)
	{
		Guid.TryParse("8408511A-CBBE-4770-8145-ED01EF1741BC", out var currentUserId);
		return handler.Handle(currentUserId, request.Prompt, cancellationToken);
	}

	internal record Request(string Prompt);
}