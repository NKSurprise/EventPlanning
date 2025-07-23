using EventPlanningAndManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Administrator")]
public class AdminController : BaseController
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> PendingRegistrations()
    {
        var pending = await _adminService.GetPendingRegistrationsAsync();
        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmRegistration(int id)
    {
        await _adminService.ConfirmRegistrationAsync(id);
        return RedirectToAction(nameof(PendingRegistrations));
    }

    [HttpPost]
    public async Task<IActionResult> DenyRegistration(int id)
    {
        await _adminService.DenyRegistrationAsync(id);
        return RedirectToAction(nameof(PendingRegistrations));
    }
}
