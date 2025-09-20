using AICalendar.ApiService.Application.Events.Create;
using AICalendar.ApiService.Application.Events.Delete;
using AICalendar.ApiService.Application.Events.Get;

namespace AICalendar.ApiService.Application.Events;

internal static class EventsModule
{
	public static IEndpointRouteBuilder MapEventRoutes(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/v1/events")
		                  .WithTags("Events")
						  .RequireAuthorization();

		group.MapCreateEvent();
		group.MapGetEvent();
		group.MapDeleteEvent();
		group.MapCurrentUserEvents();

		return group;
	}
}