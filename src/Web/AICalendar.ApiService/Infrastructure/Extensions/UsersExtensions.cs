using System.Security.Claims;
using AICalendar.ApiService.Application.User.Delete;
using AICalendar.ApiService.Application.User.Get;
using Microsoft.Identity.Web;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class UsersExtensions
{
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder builder)
    {
        builder.AddGet();
        builder.AddCreate();
        builder.AddDelete();
        return builder;
    }

    private static WebApplicationBuilder AddGet(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GetUserHandler>();
        return builder;
    }

    private static WebApplicationBuilder AddCreate(this WebApplicationBuilder builder)
    {
       // builder.Services.AddScoped<CreateUsersHandler>();
        return builder;
    }

    private static WebApplicationBuilder AddDelete(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DeleteUsersHandler>();
        return builder;
    }

	public static Guid GetUserId(this ClaimsPrincipal claims)
	{
		Guid.TryParse(claims.GetObjectId(), out var currentUserId);
		return currentUserId;
	}
}
