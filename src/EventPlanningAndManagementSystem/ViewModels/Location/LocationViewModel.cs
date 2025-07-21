using System.ComponentModel.DataAnnotations;

namespace EventPlanningAndManagementSystem.ViewModels.Location
{
    public class LocationViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Location Name")]
        public string Name { get; set; } = null!;
    }

}
