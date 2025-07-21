using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        // FK to Identity User
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public DateTime? BirthDate { get; set; }    
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
