namespace AICalendar.ApiService.Application.AI;

internal static class AiModule
{
	public static IEndpointRouteBuilder MapAiRoutes(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/v1/ai")
		                  .WithTags("AI")
						  .RequireAuthorization();

		group.MapAi();

		return group;
	}
}