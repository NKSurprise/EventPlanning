using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPlanningAndManagementSystem.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(e => e.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(e => e.PublishedOn)
                   .IsRequired();

            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false);

            // Relationships

            builder.HasOne(e => e.Category)
                   .WithMany()
                   .HasForeignKey(e => e.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if referenced

            builder.HasOne(e => e.Location)
                   .WithMany()
                   .HasForeignKey(e => e.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Publisher)
                   .WithMany() // IdentityUser does not contain ICollection<Event>
                   .HasForeignKey(e => e.PublisherId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of Publisher if referenced
        }
    }
}
