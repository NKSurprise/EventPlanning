using EventPlanningAndManagementSystem.ViewModels.Registration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanningAndManagementSystem.ViewModels.Event
{
    public class EventIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;   
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int LocationId { get; set; }
        public DateTime PublishedOn { get; set; }
        public SelectList? Categories { get; set; }
        public SelectList? Locations { get; set; }

        public ICollection<RegistrationViewModel> Registrations { get; set; } = new List<RegistrationViewModel>();

    }
}
