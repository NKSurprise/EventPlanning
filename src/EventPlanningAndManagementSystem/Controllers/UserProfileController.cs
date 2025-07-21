
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanningAndManagementSystem.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UserProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.UsersProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

            if (profile == null) return RedirectToAction("Edit");

            var model = new UserProfileViewModel
            {
                FullName = profile.FullName,
                Address = profile.Address,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                CreatedAt = profile.CreatedAt
            };

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.UsersProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

            if (profile == null)
            {
                return View(new EditUserProfileViewModel());
            }

            return View(new EditUserProfileViewModel
            {
                FullName = profile.FullName,
                Address = profile.Address,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                ExistingProfilePictureUrl = profile.ProfilePictureUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            var profile = await _context.UsersProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { UserId = userId, CreatedAt = DateTime.Now };
                _context.UsersProfiles.Add(profile);
            }

            profile.FullName = model.FullName;
            profile.Address = model.Address;
            profile.Bio = model.Bio;
            profile.BirthDate = model.BirthDate;

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
                var filePath = Path.Combine("wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                profile.ProfilePictureUrl = "/uploads/" + fileName;
            }


            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

