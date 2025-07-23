using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.UserProfile;
using Microsoft.EntityFrameworkCore;

public class UserProfileService : IUserProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public UserProfileService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<UserProfileViewModel?> GetProfileAsync(string userId)
    {
        var profile = await _context.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return null;

        return new UserProfileViewModel
        {
            FullName = profile.FullName,
            Address = profile.Address,
            Bio = profile.Bio,
            BirthDate = profile.BirthDate,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            CreatedAt = profile.CreatedAt
        };
    }

    public async Task<EditUserProfileViewModel> GetProfileForEditAsync(string userId)
    {
        var profile = await _context.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return new EditUserProfileViewModel();

        return new EditUserProfileViewModel
        {
            FullName = profile.FullName,
            Address = profile.Address,
            Bio = profile.Bio,
            BirthDate = profile.BirthDate,
            ExistingProfilePictureUrl = profile.ProfilePictureUrl
        };
    }

    public async Task SaveProfileAsync(string userId, EditUserProfileViewModel model, IFormFile? profilePicture)
    {
        var profile = await _context.UsersProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            profile = new UserProfile { UserId = userId, CreatedAt = DateTime.UtcNow };
            _context.UsersProfiles.Add(profile);
        }

        profile.FullName = model.FullName;
        profile.Address = model.Address;
        profile.Bio = model.Bio;
        profile.BirthDate = model.BirthDate;

        if (profilePicture != null && profilePicture.Length > 0)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(profilePicture.FileName);
            var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await profilePicture.CopyToAsync(stream);

            profile.ProfilePictureUrl = "/uploads/" + fileName;
        }

        await _context.SaveChangesAsync();
    }
}
