using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.ViewModels.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;

[Authorize(Roles = "Administrator")]
public class LocationController : BaseController
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var locations = await _locationService.GetAllAsync();
        return View(locations);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(AddLocationViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _locationService.LocationExistsAsync(model.Name))
        {
            ModelState.AddModelError("Name", "This location already exists.");
            return View(model);
        }

        await _locationService.AddLocationAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null) return NotFound();
        return View(location);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Location location)
    {
        if (id != location.Id) return BadRequest();
        if (!ModelState.IsValid) return View(location);

        await _locationService.UpdateLocationAsync(location);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _locationService.DeleteLocationAsync(id);
        if (!success)
        {
            TempData["ErrorMessage"] = "Cannot delete this location because it's still referenced by active events.";
            return RedirectToAction(nameof(ErrorDelete));
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult ErrorDelete() => View();
}
