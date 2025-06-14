using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.User.Update;

internal sealed class UpdateUsersHandler(AICaledandarDbContext context)
{
	public async Task<Result<UpdateUser.ResponseContent>> Handle(UpdateUser.Request request, CancellationToken cancellationToken = default)
	{
		var user = await context.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (user is null)
		{
			return Result.Failure<UpdateUser.ResponseContent>(UserErrors.NotFound(request.Id));
		}

		await context.SaveChangesAsync(cancellationToken);
		return Result.Success(new UpdateUser.ResponseContent(user.Id));
	}
}