using EventPlanningAndManagementSystem.ViewModels.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserProfileService _profileService;
    private readonly UserManager<IdentityUser> _userManager;

    public UserProfileController(IUserProfileService profileService, UserManager<IdentityUser> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var profile = await _profileService.GetProfileAsync(userId);

        if (profile == null) return RedirectToAction("Edit");
        return View(profile);
    }

    public async Task<IActionResult> Edit()
    {
        var userId = _userManager.GetUserId(User);
        var model = await _profileService.GetProfileForEditAsync(userId);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = _userManager.GetUserId(User);
        await _profileService.SaveProfileAsync(userId, model, model.ProfilePicture);

        return RedirectToAction("Index");
    }
}
