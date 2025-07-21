using Microsoft.AspNetCore.Mvc.Rendering;

public class EditEventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public DateTime PublishedOn { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Locations { get; set; }
}
