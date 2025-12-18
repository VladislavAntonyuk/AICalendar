using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.PublicHolidays.Get;

internal static class GetPublicHolidays
{
	public static RouteGroupBuilder MapGetPublicHolidays(this RouteGroupBuilder routes)
	{
		routes.MapGet("/", Handler);

		return routes;
	}

	private static IResult Handler(GetPublicHolidaysHandler handler,
		CancellationToken cancellationToken)
	{
		var result = handler.Handle(cancellationToken);
		return result.Match(Results.Ok, ApiResults.Problem);
	}
}