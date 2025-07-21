namespace EventPlanningAndManagementSystem.ViewModels.Registration
{
    public class RegistrationViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = null!;
        public string UserId { get; set; } = null!; 
        public DateTime RegisteredOn { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDenied { get; set; } = false; 
        public string? Notes { get; set; } 
    }
}
