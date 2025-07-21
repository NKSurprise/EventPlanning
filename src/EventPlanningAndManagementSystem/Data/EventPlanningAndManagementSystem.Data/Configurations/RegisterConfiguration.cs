using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;

namespace EventPlanningAndManagementSystem.Data.Configurations
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => new { r.UserId, r.EventId }).IsUnique();

            builder.HasQueryFilter(r => !r.Event.IsDeleted);

            builder.HasOne(r => r.Event)
                   .WithMany(e => e.Registrations)
                   .HasForeignKey(r => r.EventId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(r => r.User)
                   .WithMany() 
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict); 

        }
    }
}
