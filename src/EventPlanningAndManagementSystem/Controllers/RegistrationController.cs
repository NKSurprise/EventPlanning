using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels;
using EventPlanningAndManagementSystem.ViewModels.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanningAndManagementSystem.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public RegistrationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRegister(int eventId)
        {
            var userId = userManager.GetUserId(User);

            var registration = await context.Registrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (registration != null)
            {
                context.Registrations.Remove(registration);
            }
            else
            {
                context.Registrations.Add(new Registration
                {
                    EventId = eventId,
                    UserId = userId,
                    RegisteredOn = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", new { id = eventId });
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = userManager.GetUserId(User);

                var registrations = await context.Registrations
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Event)
                    .Select(r => new RegistrationViewModel
                    {
                        EventId = r.EventId,
                        EventName = r.Event.Name,
                        RegisteredOn = r.RegisteredOn,
                        IsConfirmed = r.IsConfirmed,
                        UserId = r.UserId,
                    })
                    .ToListAsync();

                return View(registrations);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while retrieving your registrations. Please try again later.");
                return View(new List<RegistrationViewModel>());
            }
        }


    }

}
