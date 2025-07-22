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
    public async Task<IActionResult> Edit(int id, Location location)
    {
        if (id != location.Id) return BadRequest();

        if (!ModelState.IsValid) return View(location);

        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound();

        // Delete all soft-deleted events that reference this location
        var softDeletedEvents = await _context.Events
            .Where(e => e.LocationId == id && e.IsDeleted)
            .ToListAsync();

        _context.Events.RemoveRange(softDeletedEvents);

        try
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Cannot delete this location because it's still referenced by active events.";
            return RedirectToAction("ErrorDelete");
        }
    }


    public IActionResult ErrorDelete()
    {
        return View();
    }


}
