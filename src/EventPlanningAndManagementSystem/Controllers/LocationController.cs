using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels;
using EventPlanningAndManagementSystem.ViewModels.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[Authorize(Roles = "Administrator")]
public class LocationController : BaseController
{
    private readonly ApplicationDbContext _context;

    public LocationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var Locations = await _context.Locations.ToListAsync();
        return View(Locations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddLocationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool exists = await _context.Locations
            .AnyAsync(l => l.Name.ToLower() == model.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Name", "This location already exists.");
            return View(model);
        }

        var location = new Location
        {
            Name = model.Name
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Edit(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        return View(location);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Location location)
    {
        if (id != location.Id) return BadRequest();

        if (!ModelState.IsValid) return View(location);

        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Location? location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
