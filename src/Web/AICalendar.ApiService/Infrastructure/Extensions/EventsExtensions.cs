using AICalendar.ApiService.Application.Events.Create;
using AICalendar.ApiService.Application.Events.Delete;
using AICalendar.ApiService.Application.Events.Get;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class EventsExtensions
{
	public static WebApplicationBuilder AddEvents(this WebApplicationBuilder builder)
	{
		builder.AddCreate();
		builder.AddGet();
		builder.AddDelete();
		return builder;
	}

	private static WebApplicationBuilder AddCreate(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<CreateEventHandler>();
		return builder;
	}

	private static WebApplicationBuilder AddGet(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<GetEventHandler>();
		builder.Services.AddScoped<GetUserEventsHandler>();
		return builder;
	}

	private static WebApplicationBuilder AddDelete(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<DeleteEventHandler>();
		return builder;
	}
}
