using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.ViewModels.AdminRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrator")]
public class AdminController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> PendingRegistrations()
    {
        var pending = await _context.Registrations
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

        return View(pending);
    }
    [HttpPost]
    public async Task<IActionResult> ConfirmRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration != null)
        {
            registration.IsConfirmed = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("PendingRegistrations");
    }
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DenyRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);

        if (registration != null && !registration.IsConfirmed)
        {
            registration.IsDenied = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("PendingRegistrations");
    }


}
