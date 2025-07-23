using EventPlanningAndManagementSystem.ViewModels.UserProfile;
using System.Threading.Tasks;

public interface IUserProfileService
{
    Task<UserProfileViewModel?> GetProfileAsync(string userId);
    Task<EditUserProfileViewModel> GetProfileForEditAsync(string userId);
    Task SaveProfileAsync(string userId, EditUserProfileViewModel model, IFormFile? profilePicture);
}
