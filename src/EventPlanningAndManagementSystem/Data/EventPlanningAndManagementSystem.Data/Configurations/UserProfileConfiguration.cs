using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Primary key
            builder.HasKey(up => up.Id);

            // Properties
            builder.Property(up => up.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(up => up.Address)
                   .HasMaxLength(200);

            builder.Property(up => up.Bio)
                   .HasMaxLength(1000);

            builder.Property(up => up.ProfilePictureUrl)
                   .HasMaxLength(500);

            builder.Property(up => up.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            // One-to-one relationship with IdentityUser (no navigation on IdentityUser)
            builder.HasOne<IdentityUser>(up => up.User)
                   .WithOne()
                   .HasForeignKey<UserProfile>(up => up.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
