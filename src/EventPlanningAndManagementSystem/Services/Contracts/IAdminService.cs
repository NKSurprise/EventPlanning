using EventPlanningAndManagementSystem.ViewModels.AdminRequests;

public interface IAdminService
{
    Task<List<PendingRegistrationViewModel>> GetPendingRegistrationsAsync();
    Task<bool> ConfirmRegistrationAsync(int id);
    Task<bool> DenyRegistrationAsync(int id);
}
