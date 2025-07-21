using Microsoft.AspNetCore.Identity;

namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public string Description { get; set; }= null!;

        public string PublisherId { get; set; } = null!;

        public virtual IdentityUser Publisher {get; set; } = null!;

        public DateTime PublishedOn { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        public int LocationId { get; set; }

        public virtual Location Location  { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public virtual ICollection<Registration> Registrations { get; set; } = new HashSet<Registration>();

    }
}
