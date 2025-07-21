using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Configurations;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<Event> Events { get; set; } = null!;

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Location> Locations { get; set; } = null!;

        public virtual DbSet<Registration> Registrations { get; set; }

        public virtual DbSet<UserProfile> UsersProfiles { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
