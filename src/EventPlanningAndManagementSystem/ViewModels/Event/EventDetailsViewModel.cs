namespace EventPlanningAndManagementSystem.ViewModels.Event
{
    public class EventDetailsViewModel : EventIndexViewModel
    {
        public string Description { get; set; } = null!;

        public string LocationName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;   
        public string? PublisherName { get; set; }
        public string PublishedOn { get; set; } = null!;

        public bool IsConfirmed { get; set; }
        public bool IsUserRegistered { get; set; }

    }
}
