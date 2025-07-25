using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class RegistrationServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"RegDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ToggleRegistrationAsync_ShouldAdd_WhenNoneExists()
        {
            var db = GetDbContext();
            db.Events.Add(new Event
            {
                Id = 1,
                Name = "Event1",
                Description = "Desc",
                PublisherId = "admin",
                CategoryId = 1,
                LocationId = 1,
                PublishedOn = DateTime.Now
            });
            db.Categories.Add(new Category { Id = 1, Name = "General" });
            db.Locations.Add(new Location { Id = 1, Name = "Virtual" });
            await db.SaveChangesAsync();

            var service = new RegistrationService(db);

            await service.ToggleRegistrationAsync(1, "user-a");

            var reg = await db.Registrations.FirstOrDefaultAsync();
            reg.Should().NotBeNull();
            reg!.UserId.Should().Be("user-a");
            reg.EventId.Should().Be(1);
        }

        [Fact]
        public async Task ToggleRegistrationAsync_ShouldRemove_WhenExists()
        {
            var db = GetDbContext();
            db.Registrations.Add(new Registration
            {
                EventId = 2,
                UserId = "user-x",
                RegisteredOn = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var service = new RegistrationService(db);
            await service.ToggleRegistrationAsync(2, "user-x");

            var reg = await db.Registrations.FirstOrDefaultAsync();
            reg.Should().BeNull();
        }

        [Fact]
        public async Task GetUserRegistrationsAsync_ShouldReturnCorrectList()
        {
            var db = GetDbContext();
            db.Events.Add(new Event
            {
                Id = 3,
                Name = "E3",
                Description = "Event3",
                PublisherId = "uid",
                CategoryId = 1,
                LocationId = 1,
                PublishedOn = DateTime.UtcNow
            });
            db.Registrations.Add(new Registration
            {
                EventId = 3,
                UserId = "user-b",
                RegisteredOn = DateTime.UtcNow,
                IsConfirmed = true
            });
            db.Categories.Add(new Category { Id = 1, Name = "Category" });
            db.Locations.Add(new Location { Id = 1, Name = "Loc" });
            await db.SaveChangesAsync();

            var service = new RegistrationService(db);
            var result = await service.GetUserRegistrationsAsync("user-b");

            result.Should().HaveCount(1);
            result[0].EventId.Should().Be(3);
            result[0].IsConfirmed.Should().BeTrue();
        }

    }
}
