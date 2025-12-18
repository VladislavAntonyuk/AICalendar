using AICalendar.ApiService.Application.PublicHolidays.Get;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class PublicHolidaysExtensions
{
	public static WebApplicationBuilder AddPublicHolidays(this WebApplicationBuilder builder)
	{
		builder.AddGet();
		return builder;
	}

	private static WebApplicationBuilder AddGet(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<GetPublicHolidaysHandler>();
		return builder;
	}
}