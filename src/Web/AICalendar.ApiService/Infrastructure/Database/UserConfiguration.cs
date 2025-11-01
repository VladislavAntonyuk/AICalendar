using AICalendar.ApiService.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICalendar.ApiService.Infrastructure.Database;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(u => u.Id);
		builder.HasIndex(x => x.Email).IsUnique();

		builder
			.HasMany(u => u.Events)
			.WithOne(e => e.Organizer)
			.HasForeignKey(e => e.OrganizerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasData(new User
		{
			Id = Guid.Parse("4012d03e-edc0-42f1-8003-5bad4ec5c9b1"),
			Email = "vlad.antonyuk@gmail.com"
		});
	}
}