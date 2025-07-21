using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public string UserId { get; set; } 
        public virtual IdentityUser User { get; set; } = null!;

        public DateTime RegisteredOn { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; } = false;
        public string? Notes { get; set; } 

    }

}
