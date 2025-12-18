using AICalendar.ApiService.Application.PublicHolidays.Get;

namespace AICalendar.ApiService.Application.PublicHolidays;

internal static class PublicHolidaysModule
{
	public static IEndpointRouteBuilder MapPublicHolidaysRoutes(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/v1/public_holidays")
		                  .WithTags("Public Holidays");

		group.MapGetPublicHolidays();

		return group;
	}
}