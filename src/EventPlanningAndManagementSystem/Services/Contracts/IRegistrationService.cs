// IRegistrationService.cs
using EventPlanningAndManagementSystem.ViewModels.Registration;

public interface IRegistrationService
{
    Task ToggleRegistrationAsync(int eventId, string userId);
    Task<List<RegistrationViewModel>> GetUserRegistrationsAsync(string userId);
}
