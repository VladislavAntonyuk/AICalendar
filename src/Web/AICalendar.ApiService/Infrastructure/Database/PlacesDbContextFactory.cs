using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AICalendar.ApiService.Infrastructure.Database;

public class PlacesDbContextFactory : IDesignTimeDbContextFactory<AiCalendarDbContext>
{
	public AiCalendarDbContext CreateDbContext(string[] args)
	{
		return new AiCalendarDbContext(new DbContextOptionsBuilder<AiCalendarDbContext>()
		                               .UseSqlServer("Host=localhost;Database=ai-calendar;Username=sa;Password=password")
		                               .Options);
	}
}