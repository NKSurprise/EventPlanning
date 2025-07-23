using EventPlanningAndManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class RegistrationController : BaseController
{
    private readonly IRegistrationService _registrationService;
    private readonly UserManager<IdentityUser> _userManager;

    public RegistrationController(IRegistrationService registrationService, UserManager<IdentityUser> userManager)
    {
        _registrationService = registrationService;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleRegister(int eventId)
    {
        var userId = _userManager.GetUserId(User);
        await _registrationService.ToggleRegistrationAsync(eventId, userId);
        return RedirectToAction("Details", "Events", new { id = eventId });
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var registrations = await _registrationService.GetUserRegistrationsAsync(userId);
        return View(registrations);
    }
}
