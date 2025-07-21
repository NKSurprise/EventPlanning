using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models
{
    public class Registration
    {
        public int Id { get; set; }

        // Foreign Key to the Event
        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        // Foreign Key to the User
        public string UserId { get; set; } 
        public virtual IdentityUser User { get; set; } = null!;

        // Extra registration details
        public DateTime RegisteredOn { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; } = false;
        public string? Notes { get; set; } // Special requests, dietary, etc.

    }

}
