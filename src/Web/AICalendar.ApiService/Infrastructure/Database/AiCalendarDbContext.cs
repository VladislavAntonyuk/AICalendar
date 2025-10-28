using AICalendar.ApiService.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Infrastructure.Database;

public sealed class AiCalendarDbContext(DbContextOptions<AiCalendarDbContext> options) : DbContext(options)
{
	internal DbSet<User> Users => Set<User>();
	internal DbSet<CalendarEvent> Events => Set<CalendarEvent>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new UserConfiguration());
	}
}

#if DEBUG
// dotnet ef migrations add "Initial" -o "Infrastructure/Database/Migrations"
#endif
