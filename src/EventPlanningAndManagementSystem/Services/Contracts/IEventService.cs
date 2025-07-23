using EventPlanningAndManagementSystem.ViewModels.Event;
using Microsoft.AspNetCore.Mvc.Rendering;

public interface IEventService
{
    Task<IEnumerable<EventIndexViewModel>> GetAllEventsAsync(string? userId, string? search = null, int? categoryId = null);
    Task<EventDetailsViewModel?> GetEventDetailsAsync(int? id, string? userId);
    Task<bool> AddEventAsync(string userId, AddEventInputModel inputModel);
    Task<EditEventViewModel?> GetEventForEditAsync(string userId, int? id);
    Task<EditEventViewModel> PrepareEditEventModelAsync(EditEventViewModel model);
    Task<AddEventInputModel> PrepareNewEventFormModelAsync();
    Task<AddEventInputModel> PrepareNewEventFormModelAsync(AddEventInputModel model);
    Task<bool> PersistUpdatedEventAsync(string userId, EditEventViewModel inputModel);
    Task<bool> DeleteEventAsync(int eventId, string userId);
    Task<SelectList> GetCategorySelectListAsync(int? selectedId = null);

}
