using System.Security.Claims;
using AICalendar.ApiService.Infrastructure.Extensions;
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
		ClaimsPrincipal claims,
		Request request,
		CancellationToken cancellationToken)
	{
		return handler.Handle(claims.GetUserId(), request.Prompt, cancellationToken);
	}

	internal record Request(string Prompt);
}