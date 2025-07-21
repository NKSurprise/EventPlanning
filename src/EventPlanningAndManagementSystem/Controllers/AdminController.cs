using EventPlanningAndManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Promote a user by ID
    [HttpPost]
    public async Task<IActionResult> PromoteToAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Ensure the Admin role exists
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Assign user to Admin role
        var result = await _userManager.AddToRoleAsync(user, "Admin");

        if (result.Succeeded)
        {
            TempData["Success"] = "User promoted to admin.";
        }
        else
        {
            TempData["Error"] = "Failed to promote user.";
        }

        return RedirectToAction("Users"); // Or wherever your user list is
    }
}
