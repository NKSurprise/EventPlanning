using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.UserProfile;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventPlanningAndManagementSystem.Tests.Services
{
    public class UserProfileServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("UserProfileDb_" + Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        private IWebHostEnvironment GetMockEnvironment()
        {
            var env = new Mock<IWebHostEnvironment>();
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);
            env.Setup(e => e.WebRootPath).Returns(tempPath);
            return env.Object;
        }

        [Fact]
        public async Task GetProfileAsync_ShouldReturnProfile_WhenExists()
        {
            var db = GetDbContext();
            db.UsersProfiles.Add(new UserProfile
            {
                UserId = "user-1",
                FullName = "John",
                Address = "123 St",
                Bio = "About",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var service = new UserProfileService(db, GetMockEnvironment());
            var result = await service.GetProfileAsync("user-1");

            result.Should().NotBeNull();
            result!.FullName.Should().Be("John");
        }

        [Fact]
        public async Task GetProfileForEditAsync_ShouldReturnEmpty_WhenNotFound()
        {
            var db = GetDbContext();
            var service = new UserProfileService(db, GetMockEnvironment());

            var model = await service.GetProfileForEditAsync("non-existent");

            model.Should().NotBeNull();
            model.FullName.Should().BeNull(); // empty object
        }

        [Fact]
        public async Task SaveProfileAsync_ShouldCreateNewProfile()
        {
            var db = GetDbContext();
            var env = GetMockEnvironment();
            var service = new UserProfileService(db, env);

            var model = new EditUserProfileViewModel
            {
                FullName = "New User",
                Address = "Somewhere",
                Bio = "Bio",
                BirthDate = new DateTime(2000, 1, 1)
            };

            await service.SaveProfileAsync("user-2", model, null);

            var profile = await db.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == "user-2");
            profile.Should().NotBeNull();
            profile!.FullName.Should().Be("New User");
        }

        [Fact]
        public async Task SaveProfileAsync_ShouldUpdateExistingProfile()
        {
            var db = GetDbContext();
            db.UsersProfiles.Add(new UserProfile
            {
                UserId = "user-3",
                FullName = "Old Name",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var service = new UserProfileService(db, GetMockEnvironment());

            var model = new EditUserProfileViewModel
            {
                FullName = "Updated",
                Address = "New Addr",
                Bio = "New Bio",
                BirthDate = new DateTime(1990, 1, 1)
            };

            await service.SaveProfileAsync("user-3", model, null);

            var updated = await db.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == "user-3");
            updated!.FullName.Should().Be("Updated");
            updated.Bio.Should().Be("New Bio");
        }

        [Fact]
        public async Task SaveProfileAsync_ShouldHandleProfilePictureUpload()
        {
            // Arrange
            var db = GetDbContext();

            // Create a temporary fake web root path
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempRoot, "uploads"));

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(tempRoot);

            var service = new UserProfileService(db, envMock.Object);

            var content = new byte[] { 1, 2, 3 };
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var ms = new MemoryStream(content);

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(content.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns((Stream stream, CancellationToken _) =>
                    {
                        return ms.CopyToAsync(stream);
                    });

            var model = new EditUserProfileViewModel
            {
                FullName = "PicUser"
            };

            // Act
            await service.SaveProfileAsync("user-4", model, fileMock.Object);

            // Assert
            var profile = await db.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == "user-4");
            profile.Should().NotBeNull();
            profile!.ProfilePictureUrl.Should().StartWith("/uploads/");

            // Cleanup temp directory
            Directory.Delete(tempRoot, recursive: true);
        }
    }
}
