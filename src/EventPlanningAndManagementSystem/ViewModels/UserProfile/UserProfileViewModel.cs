namespace EventPlanningAndManagementSystem.ViewModels.UserProfile
{
    public class UserProfileViewModel
    {
        public string FullName { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
