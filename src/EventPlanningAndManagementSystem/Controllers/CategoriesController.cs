using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Administrator")]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(AddCategoryViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _categoryService.AddCategoryAsync(model);
        if (!success)
        {
            ModelState.AddModelError("Name", "This category already exists.");
            return View(model);
        }

        return RedirectToAction("Index", "Events");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category == null ? NotFound() : View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (!ModelState.IsValid) return View(category);

        await _categoryService.UpdateCategoryAsync(category);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        if (!success)
        {
            TempData["ErrorMessage"] = "Cannot delete this category because it's still referenced by active events.";
            return RedirectToAction("ErrorDelete");
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult ErrorDelete() => View();
}
