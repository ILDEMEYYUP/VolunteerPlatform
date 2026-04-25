using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Many-to-many relationship for RecommendedProjects
        builder.HasMany(u => u.RecommendedProjects)
            .WithMany()
            .UsingEntity("UserRecommendedProjects");
    }
}
