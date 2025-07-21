using EventPlanningAndManagementSystem.ViewModels.Event;

public interface IEventService
{
    Task<IEnumerable<EventIndexViewModel>> GetAllEventsAsync(
    string? userId,
    string? search = null,
    int? categoryId = null);

    Task<EventDetailsViewModel?> GetEventDetailsAsync(int? id, string? userId);
    Task<bool> AddEventAsync(string userId, AddEventInputModel inputModel);
    Task<EditEventViewModel?> GetEventForEditAsync(string userId, int? id);
    Task<bool> PersistUpdatedEventAsync(string userId, EditEventViewModel inputModel);
}
