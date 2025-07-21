namespace EventPlanningAndManagementSystem.ViewModels.AdminRequests
{
    public class PendingRegistrationViewModel
    {
        public int RegistrationId { get; set; }
        public string EventName { get; set; }
        public string UserEmail { get; set; }
        public DateTime RegisteredOn { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
