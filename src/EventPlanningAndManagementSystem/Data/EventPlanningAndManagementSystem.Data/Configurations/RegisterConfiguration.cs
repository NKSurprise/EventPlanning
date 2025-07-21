using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;

namespace EventPlanningAndManagementSystem.Data.Configurations
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            // Primary key
            builder.HasKey(r => r.Id);

            // Composite unique constraint to prevent duplicate registrations
            builder.HasIndex(r => new { r.UserId, r.EventId }).IsUnique();

            // Optional global query filter (e.g., hide deleted events)
            builder.HasQueryFilter(r => !r.Event.IsDeleted);

            // Configure relationship to Event
            builder.HasOne(r => r.Event)
                   .WithMany(e => e.Registrations)
                   .HasForeignKey(r => r.EventId)
                   .OnDelete(DeleteBehavior.Cascade); // When event is deleted, cascade

            // Configure relationship to User (no nav collection in IdentityUser)
            builder.HasOne(r => r.User)
                   .WithMany() // if you ever add ICollection<Registration> to the user, adjust here
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting users if they have registrations

            // Property constraints
            builder.Property(r => r.Notes)
                   .HasMaxLength(500);
        }
    }
}
