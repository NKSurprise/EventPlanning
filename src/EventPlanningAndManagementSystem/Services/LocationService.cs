// LocationService.cs
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Location;
using Microsoft.EntityFrameworkCore;

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;

    public LocationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Location>> GetAllAsync() =>
        await _context.Locations.ToListAsync();

    public async Task<bool> LocationExistsAsync(string name) =>
        await _context.Locations.AnyAsync(l => l.Name.ToLower() == name.ToLower());

    public async Task AddLocationAsync(AddLocationViewModel model)
    {
        var location = new Location { Name = model.Name };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
    }

    public async Task<Location?> GetByIdAsync(int id) =>
        await _context.Locations.FindAsync(id);

    public async Task UpdateLocationAsync(Location location)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return false;

        var softDeletedEvents = await _context.Events
            .Where(e => e.LocationId == id && e.IsDeleted)
            .ToListAsync();

        _context.Events.RemoveRange(softDeletedEvents);

        try
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
