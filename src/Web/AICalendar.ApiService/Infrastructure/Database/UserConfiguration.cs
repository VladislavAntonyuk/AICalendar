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
			Id = Guid.Parse("8408511A-CBBE-4770-8145-ED01EF1741BC"),
			Email = "vlad.antonyuk@gmail.com"
		});
	}
}