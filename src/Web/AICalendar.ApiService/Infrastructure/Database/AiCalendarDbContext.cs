using AICalendar.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
public class PlacesDbContextFactory : IDesignTimeDbContextFactory<AiCalendarDbContext>
{
	public AiCalendarDbContext CreateDbContext(string[] args)
	{
		return new AiCalendarDbContext(new DbContextOptionsBuilder<AiCalendarDbContext>()
		                               .UseSqlServer("Host=localhost;Database=ai-calendar;Username=sa;Password=password")
		                               .Options);
	}
}
#endif
