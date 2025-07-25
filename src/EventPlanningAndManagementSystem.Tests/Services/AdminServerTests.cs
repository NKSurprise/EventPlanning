using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class AdminServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetPendingRegistrationsAsync_ShouldReturnOnlyUnconfirmedUndenied()
        {
            var db = GetDbContext();

            var reg1 = new Registration { Id = 1, UserId = "u1", EventId = 1, IsConfirmed = false, IsDenied = false, RegisteredOn = DateTime.UtcNow };
            var reg2 = new Registration { Id = 2, UserId = "u2", EventId = 1, IsConfirmed = true, IsDenied = false };
            var reg3 = new Registration { Id = 3, UserId = "u3", EventId = 1, IsConfirmed = false, IsDenied = true };

            db.Users.AddRange(new IdentityUser { Id = "u1", Email = "u1@mail.com" }, new IdentityUser { Id = "u2" }, new IdentityUser { Id = "u3" });
            db.Events.Add(new Event { Id = 1, Name = "Sample Event", Description = "desc", PublisherId = "u1", CategoryId = 1, LocationId = 1, PublishedOn = DateTime.Now });
            db.Categories.Add(new Category { Id = 1, Name = "C" });
            db.Locations.Add(new Location { Id = 1, Name = "L" });
            db.Registrations.AddRange(reg1, reg2, reg3);
            await db.SaveChangesAsync();

            var service = new AdminService(db);
            var result = await service.GetPendingRegistrationsAsync();

            result.Should().HaveCount(1);
            result.First().RegistrationId.Should().Be(1);
        }

        [Fact]
        public async Task ConfirmRegistrationAsync_ShouldSetIsConfirmed()
        {
            var db = GetDbContext();
            db.Users.Add(new IdentityUser { Id = "user1", Email = "user1@mail.com" });

            db.Registrations.Add(new Registration
            {
                Id = 5,
                UserId = "user1", // ✅ REQUIRED
                EventId = 1,
                IsConfirmed = false,
                IsDenied = false
            });

            db.Events.Add(new Event
            {
                Id = 1,
                Name = "Event",
                Description = "desc",
                PublisherId = "user1",
                CategoryId = 1,
                LocationId = 1,
                PublishedOn = DateTime.Now
            });

            db.Categories.Add(new Category { Id = 1, Name = "Cat" });
            db.Locations.Add(new Location { Id = 1, Name = "Loc" });

            await db.SaveChangesAsync();

            var service = new AdminService(db);
            var success = await service.ConfirmRegistrationAsync(5);

            success.Should().BeTrue();
            var reg = await db.Registrations.FindAsync(5);
            reg!.IsConfirmed.Should().BeTrue();
        }


        [Fact]
        public async Task DenyRegistrationAsync_ShouldSetIsDenied()
        {
            var db = GetDbContext();
            db.Users.Add(new IdentityUser { Id = "user2", Email = "user2@mail.com" });

            db.Registrations.Add(new Registration
            {
                Id = 6,
                UserId = "user2", // ✅ REQUIRED
                EventId = 1,
                IsConfirmed = false,
                IsDenied = false
            });

            db.Events.Add(new Event
            {
                Id = 1,
                Name = "Event",
                Description = "desc",
                PublisherId = "user2",
                CategoryId = 1,
                LocationId = 1,
                PublishedOn = DateTime.Now
            });

            db.Categories.Add(new Category { Id = 1, Name = "Cat" });
            db.Locations.Add(new Location { Id = 1, Name = "Loc" });

            await db.SaveChangesAsync();

            var service = new AdminService(db);
            var success = await service.DenyRegistrationAsync(6);

            success.Should().BeTrue();
            var reg = await db.Registrations.FindAsync(6);
            reg!.IsDenied.Should().BeTrue();
        }

        [Fact]
        public async Task DenyRegistrationAsync_ShouldReturnFalseIfAlreadyConfirmed()
        {
            var db = GetDbContext();
            db.Users.Add(new IdentityUser { Id = "user3", Email = "user3@mail.com" });

            db.Registrations.Add(new Registration
            {
                Id = 7,
                UserId = "user3", // ✅ REQUIRED
                EventId = 1,
                IsConfirmed = true,
                IsDenied = false
            });

            db.Events.Add(new Event
            {
                Id = 1,
                Name = "Event",
                Description = "desc",
                PublisherId = "user3",
                CategoryId = 1,
                LocationId = 1,
                PublishedOn = DateTime.Now
            });

            db.Categories.Add(new Category { Id = 1, Name = "Cat" });
            db.Locations.Add(new Location { Id = 1, Name = "Loc" });

            await db.SaveChangesAsync();

            var service = new AdminService(db);
            var result = await service.DenyRegistrationAsync(7);

            result.Should().BeFalse();
        }


    }
}
