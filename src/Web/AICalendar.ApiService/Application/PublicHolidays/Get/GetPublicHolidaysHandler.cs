using AICalendar.ApiService.Infrastructure.Results;

namespace AICalendar.ApiService.Application.PublicHolidays.Get;

internal sealed class GetPublicHolidaysHandler()
{
	public Result<List<string>> Handle(CancellationToken cancellationToken = default)
	{
		return Result.Success<List<string>>([
			"Christmas Day",
			"New Year's Day"]);
	}
}