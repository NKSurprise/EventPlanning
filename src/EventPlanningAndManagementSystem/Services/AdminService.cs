using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.ViewModels.AdminRequests;
using Microsoft.EntityFrameworkCore;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PendingRegistrationViewModel>> GetPendingRegistrationsAsync()
    {
        return await _context.Registrations
            .Where(r => !r.IsConfirmed && !r.IsDenied)
            .Include(r => r.Event)
            .Include(r => r.User)
            .Select(r => new PendingRegistrationViewModel
            {
                RegistrationId = r.Id,
                EventName = r.Event.Name,
                UserEmail = r.User.Email,
                RegisteredOn = r.RegisteredOn,
                IsConfirmed = r.IsConfirmed
            })
            .ToListAsync();
    }

    public async Task<bool> ConfirmRegistrationAsync(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null) return false;

        registration.IsConfirmed = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DenyRegistrationAsync(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null || registration.IsConfirmed) return false;

        registration.IsDenied = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
