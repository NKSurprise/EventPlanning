// ILocationService.cs
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Location;

public interface ILocationService
{
    Task<List<Location>> GetAllAsync();
    Task<bool> LocationExistsAsync(string name);
    Task AddLocationAsync(AddLocationViewModel model);
    Task<Location?> GetByIdAsync(int id);
    Task UpdateLocationAsync(Location location);
    Task<bool> DeleteLocationAsync(int id);
}
