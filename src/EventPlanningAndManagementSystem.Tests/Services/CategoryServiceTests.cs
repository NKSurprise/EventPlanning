using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Category;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class CategoryServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"CategoryDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var db = GetDbContext();
            db.Categories.AddRange(
                new Category { Id = 1, Name = "Tech" },
                new Category { Id = 2, Name = "Music" }
            );
            await db.SaveChangesAsync();

            var service = new CategoryService(db);
            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldReturnFalseIfCategoryExists()
        {
            var db = GetDbContext();
            db.Categories.Add(new Category { Id = 1, Name = "Art" });
            await db.SaveChangesAsync();

            var service = new CategoryService(db);
            var result = await service.AddCategoryAsync(new AddCategoryViewModel { Name = "art" });

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldAddCategoryIfNotExists()
        {
            var db = GetDbContext();
            var service = new CategoryService(db);

            var model = new AddCategoryViewModel { Name = "Science" };
            var result = await service.AddCategoryAsync(model);

            result.Should().BeTrue();
            db.Categories.Should().ContainSingle(c => c.Name == "Science");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectCategory()
        {
            var db = GetDbContext();
            db.Categories.Add(new Category { Id = 3, Name = "Gaming" });
            await db.SaveChangesAsync();

            var service = new CategoryService(db);
            var result = await service.GetByIdAsync(3);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Gaming");
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdateCategory()
        {
            var db = GetDbContext();
            var category = new Category { Id = 4, Name = "Old" };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            category.Name = "Updated";
            var service = new CategoryService(db);
            var result = await service.UpdateCategoryAsync(category);

            result.Should().BeTrue();
            db.Categories.First(c => c.Id == 4).Name.Should().Be("Updated");
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldDeleteCategoryAndSoftDeletedEvents()
        {
            var db = GetDbContext();

            db.Categories.Add(new Category { Id = 5, Name = "ToDelete" });
            db.Events.Add(new Event
            {
                Id = 500,
                Name = "DeletedEvent",
                CategoryId = 5,
                IsDeleted = true,
                Description = "desc",
                PublisherId = "uid",
                LocationId = 1,
                PublishedOn = DateTime.UtcNow
            });
            db.Locations.Add(new Location { Id = 1, Name = "Default" });
            await db.SaveChangesAsync();

            var service = new CategoryService(db);
            var result = await service.DeleteCategoryAsync(5);

            result.Should().BeTrue();
            db.Categories.Find(5).Should().BeNull();
            db.Events.Find(500).Should().BeNull();
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnFalseIfCategoryNotFound()
        {
            var db = GetDbContext();
            var service = new CategoryService(db);

            var result = await service.DeleteCategoryAsync(999);

            result.Should().BeFalse();
        }
    }
}

