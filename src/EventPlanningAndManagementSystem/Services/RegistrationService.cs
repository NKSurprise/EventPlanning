// RegistrationService.cs
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

public class RegistrationService : IRegistrationService
{
    private readonly ApplicationDbContext _context;

    public RegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ToggleRegistrationAsync(int eventId, string userId)
    {
        var registration = await _context.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (registration != null)
        {
            _context.Registrations.Remove(registration);
        }
        else
        {
            _context.Registrations.Add(new Registration
            {
                EventId = eventId,
                UserId = userId,
                RegisteredOn = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<RegistrationViewModel>> GetUserRegistrationsAsync(string userId)
    {
        return await _context.Registrations
            .Where(r => r.UserId == userId)
            .Include(r => r.Event)
            .Select(r => new RegistrationViewModel
            {
                EventId = r.EventId,
                EventName = r.Event.Name,
                RegisteredOn = r.RegisteredOn,
                IsConfirmed = r.IsConfirmed,
                IsDenied = r.IsDenied,
                UserId = r.UserId
            })
            .ToListAsync();
    }
}
