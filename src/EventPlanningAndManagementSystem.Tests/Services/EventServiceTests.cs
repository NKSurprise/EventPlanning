using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EventPlanningAndManagementSystem.Services;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Event;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class EventServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        private UserManager<IdentityUser> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null).Object;
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnOnlyNonDeletedEvents()
        {
            // Arrange
            var db = GetDbContext();
            var userManager = GetUserManagerMock();

            db.Categories.Add(new Category { Id = 1, Name = "Tech" });
            db.Locations.Add(new Location { Id = 1, Name = "NYC" });

            db.Events.AddRange(
                new Event
                {
                    Id = 1,
                    Name = "Active Event",
                    Description = "An active test event",        // ✅ Required
                    PublisherId = "user-1",                      // ✅ Required
                    IsDeleted = false,
                    CategoryId = 1,
                    LocationId = 1,
                    PublishedOn = DateTime.Now
                },
                new Event
                {
                    Id = 2,
                    Name = "Deleted Event",
                    Description = "A soft-deleted test event",   // ✅ Required
                    PublisherId = "user-1",                      // ✅ Required
                    IsDeleted = true,
                    CategoryId = 1,
                    LocationId = 1,
                    PublishedOn = DateTime.Now
                }
            );

            await db.SaveChangesAsync();

            var service = new EventService(db, userManager);

            // Act
            var result = await service.GetAllEventsAsync(null);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Active Event");
        }
        [Fact]
        public async Task AddEventAsync_ShouldAddEventCorrectly()
        {
            // Arrange
            var db = GetDbContext();
            var userManager = GetUserManagerMock();

            var category = new Category { Id = 1, Name = "Education" };
            var location = new Location { Id = 1, Name = "Hall A" };
            db.Categories.Add(category);
            db.Locations.Add(location);
            await db.SaveChangesAsync();

            var service = new EventService(db, userManager);
            var model = new AddEventInputModel
            {
                Name = "Test Event",
                Description = "A test",
                ImageUrl = "/test.jpg",
                PublishedOn = "25-07-2025",
                CategoryId = 1,
                LocationId = 1
            };

            // Act
            var result = await service.AddEventAsync("test-user", model);

            // Assert
            result.Should().BeTrue();
            var added = db.Events.FirstOrDefault();
            added.Should().NotBeNull();
            added!.Name.Should().Be("Test Event");
        }

        [Fact]
        public async Task GetEventDetailsAsync_ShouldReturnDetailsIfExists()
        {
            // Arrange
            var db = GetDbContext();
            var userManager = GetUserManagerMock();

            var user = new IdentityUser { Id = "user-1", UserName = "john@example.com" };
            var location = new Location { Id = 1, Name = "Main Hall" };
            var category = new Category { Id = 1, Name = "Sports" };
            var ev = new Event
            {
                Id = 1,
                Name = "Marathon",
                PublisherId = user.Id,
                LocationId = location.Id,
                CategoryId = category.Id,
                Description = "Run",
                ImageUrl = "/img.jpg",
                PublishedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            db.Users.Add(user);
            db.Locations.Add(location);
            db.Categories.Add(category);
            db.Events.Add(ev);
            db.Registrations.Add(new Registration { EventId = ev.Id, UserId = user.Id });
            await db.SaveChangesAsync();

            var service = new EventService(db, userManager);

            // Act
            var result = await service.GetEventDetailsAsync(1, user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Marathon");
            result.IsUserRegistered.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldRemoveEventIfExists()
        {
            // Arrange
            var db = GetDbContext();
            var userManager = GetUserManagerMock();

            // Required related data
            db.Categories.Add(new Category { Id = 1, Name = "Tech" });
            db.Locations.Add(new Location { Id = 1, Name = "Virtual" });

            var ev = new Event
            {
                Id = 10,
                Name = "To Delete",
                Description = "This event is to be deleted", // ✅ Required
                PublisherId = "user-x",                     // ✅ Required
                CategoryId = 1,
                LocationId = 1,
                IsDeleted = false,
                PublishedOn = DateTime.UtcNow
            };

            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var service = new EventService(db, userManager);

            // Act
            var result = await service.DeleteEventAsync(10, "user-x");

            // Assert
            result.Should().BeTrue();
            var evInDb = await db.Events.FindAsync(10);
            evInDb.Should().BeNull();
        }

    }
}
