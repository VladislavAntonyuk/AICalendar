using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Application.User.Get;

internal sealed class GetUserHandler(AiCalendarDbContext context)
{
	public async Task<Result<GetUser.ResponseContent>> Handle(Guid id, CancellationToken cancellationToken = default)
	{
		var model = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		return model is null ? Result.Failure<GetUser.ResponseContent>(UserErrors.NotFound(id)) : Result.Success(new GetUser.ResponseContent(model.Id));
	}
}