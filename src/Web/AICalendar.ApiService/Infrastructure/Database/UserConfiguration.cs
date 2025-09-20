using AICalendar.Shared;
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

		builder.HasData(new User()
		{
			Id = Guid.Parse("48b828bd-8f7d-41e6-8872-02a783e6edc2"),
			Email = "vlad.antonyuk@gmail.com"
		});
	}
}