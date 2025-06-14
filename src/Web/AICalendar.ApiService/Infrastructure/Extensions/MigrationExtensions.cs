using AICalendar.ApiService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class MigrationExtensions
{
	public static async Task ApplyMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();

		await ApplyMigration<AiCalendarDbContext>(scope);
	}

	private static async Task ApplyMigration<TDbContext>(IServiceScope scope) where TDbContext : DbContext
	{
		await using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		await context.Database.MigrateAsync();
	}
}