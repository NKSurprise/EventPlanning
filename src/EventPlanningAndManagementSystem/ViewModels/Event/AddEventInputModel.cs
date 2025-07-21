using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static EventPlanningAndManagementSystem.EventPlanningAndManagementSystem.GCommon.ValidationConstatnts.Event;
public class AddEventInputModel
{
    public int Id { get; set; }
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    [Required]
    [MinLength(DescriptionMinLength)]
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    [Required]
    [MinLength(PublishedOnLength)]
    [MaxLength(PublishedOnLength)]
    public string PublishedOn { get; set; } = null!;
    public int CategoryId { get; set; }
    public int LocationId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Locations { get; set; }
}
