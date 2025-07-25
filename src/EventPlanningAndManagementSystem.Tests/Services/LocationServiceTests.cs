using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Location;
using System.Collections.Generic;
using System.Linq;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class LocationServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"LocationDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLocations()
        {
            var db = GetDbContext();
            db.Locations.AddRange(
                new Location { Id = 1, Name = "Sofia" },
                new Location { Id = 2, Name = "London" }
            );
            await db.SaveChangesAsync();

            var service = new LocationService(db);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task LocationExistsAsync_ShouldReturnTrueIfExists()
        {
            var db = GetDbContext();
            db.Locations.Add(new Location { Id = 1, Name = "Berlin" });
            await db.SaveChangesAsync();

            var service = new LocationService(db);

            var exists = await service.LocationExistsAsync("berlin");

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task AddLocationAsync_ShouldAddLocation()
        {
            var db = GetDbContext();
            var service = new LocationService(db);

            var model = new AddLocationViewModel { Name = "Tokyo" };

            await service.AddLocationAsync(model);

            db.Locations.Should().ContainSingle(l => l.Name == "Tokyo");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLocation()
        {
            var db = GetDbContext();
            db.Locations.Add(new Location { Id = 5, Name = "Paris" });
            await db.SaveChangesAsync();

            var service = new LocationService(db);

            var result = await service.GetByIdAsync(5);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Paris");
        }

        [Fact]
        public async Task UpdateLocationAsync_ShouldUpdateLocationName()
        {
            var db = GetDbContext();
            var location = new Location { Id = 3, Name = "OldName" };
            db.Locations.Add(location);
            await db.SaveChangesAsync();

            var service = new LocationService(db);
            location.Name = "NewName";

            await service.UpdateLocationAsync(location);

            db.Locations.First(l => l.Id == 3).Name.Should().Be("NewName");
        }

        [Fact]
        public async Task DeleteLocationAsync_ShouldDeleteLocationWithSoftDeletedEvents()
        {
            var db = GetDbContext();
            db.Locations.Add(new Location { Id = 10, Name = "DeleteMe" });
            db.Events.Add(new Event
            {
                Id = 100,
                Name = "SoftDeletedEvent",
                LocationId = 10,
                IsDeleted = true,
                Description = "desc",
                PublisherId = "uid",
                CategoryId = 1,
                PublishedOn = DateTime.UtcNow
            });
            db.Categories.Add(new Category { Id = 1, Name = "Tech" });
            await db.SaveChangesAsync();

            var service = new LocationService(db);

            var result = await service.DeleteLocationAsync(10);

            result.Should().BeTrue();
            db.Locations.Find(10).Should().BeNull();
            db.Events.Find(100).Should().BeNull();
        }

        [Fact]
        public async Task DeleteLocationAsync_ShouldReturnFalseIfLocationNotFound()
        {
            var db = GetDbContext();
            var service = new LocationService(db);

            var result = await service.DeleteLocationAsync(999); // ID doesn't exist

            result.Should().BeFalse();
        }
    }
}

