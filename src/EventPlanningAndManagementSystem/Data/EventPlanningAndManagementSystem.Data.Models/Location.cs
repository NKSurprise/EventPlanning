using System.ComponentModel.DataAnnotations;
using static EventPlanningAndManagementSystem.EventPlanningAndManagementSystem.GCommon.ValidationConstatnts.Location;
namespace EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models
{
    public class Location
    {
        public int Id { get; set; }

        [MaxLength(NameMaxLength)]
        [MinLength(NameMinLength)]
        public string Name { get; set; }  = null!;
    }
}
