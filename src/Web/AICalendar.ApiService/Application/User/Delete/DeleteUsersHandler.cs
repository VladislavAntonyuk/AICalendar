using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.User.Delete;

internal sealed class DeleteUsersHandler(AiCalendarDbContext context)
{
	public async Task<Result> Handle(Guid id, CancellationToken cancellationToken = default)
	{
		var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound(id));
		}

		context.Users.Remove(user);
		await context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}