using Microsoft.AspNetCore.Mvc;

namespace EventPlanningAndManagementSystem.ViewModels.UserProfile
{
    public class EditUserProfileViewModel
    {
        public string FullName { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }

        public IFormFile? ProfilePicture { get; set; } // For uploading new image

        public string? ExistingProfilePictureUrl { get; set; } // ← Add this line
    }


}
