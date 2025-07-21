using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlanningAndManagementSystem.Controllers
{
    //[Authorize(Roles = "Admin")] // Only admins can access this controller
    public class CategoriesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());

            if (exists)
            {
                ModelState.AddModelError("Name", "This category already exists.");
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Events");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();

            if (!ModelState.IsValid) return View(category);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
