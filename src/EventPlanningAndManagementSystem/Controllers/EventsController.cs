using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using EventPlanningAndManagementSystem.ViewModels.Event;

public class EventsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEventService _eventService;

    public EventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEventService eventService)
    {
        _context = context;
        _userManager = userManager;
        _eventService = eventService; 
    }


    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? page)
    {
        var userId = _userManager.GetUserId(User);
        var events = await _eventService.GetAllEventsAsync(userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            events = events.Where(e => e.Name.ToLower().Contains(searchTerm)).ToList();
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            events = events.Where(e => e.CategoryId == categoryId.Value).ToList();
        }

        ViewData["CurrentFilter"] = searchTerm;
        ViewData["CurrentCategory"] = categoryId;
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

        int pageSize = 8;
        int pageNumber = page ?? 1;

        var pagedEvents = events.ToPagedList(pageNumber, pageSize);

        return View(pagedEvents);
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        var model = new AddEventInputModel
        {
            Locations = _context.Locations
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToList(),

            Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList(),

            PublishedOn = DateTime.Now.ToString("dd-MM-yyyy")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddEventInputModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Locations = _context.Locations
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToList();

            model.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(model);
        }

        var newEvent = new Event
        {
            Name = model.Name,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            PublishedOn = DateTime.ParseExact(model.PublishedOn, "dd-MM-yyyy", null),
            CategoryId = model.CategoryId,
            LocationId = model.LocationId,
            PublisherId = _userManager.GetUserId(User)!
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        var model = new EditEventViewModel
        {
            Id = ev.Id,
            Name = ev.Name,
            ImageUrl = ev.ImageUrl,
            Description = ev.Description,
            CategoryId = ev.CategoryId,
            LocationId = ev.LocationId,
            PublishedOn = ev.PublishedOn
        };

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", ev.CategoryId);
        ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name", ev.LocationId);

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditEventViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name", model.LocationId);
            return View(model);
        }

        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        // Update allowed properties only
        ev.Name = model.Name;
        ev.Description = model.Description;
        ev.ImageUrl = model.ImageUrl;
        ev.CategoryId = model.CategoryId;
        ev.LocationId = model.LocationId;
        ev.PublishedOn = model.PublishedOn;

        try
        {
            _context.Update(ev);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error saving changes.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }


    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Location)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null) return NotFound();

        return View(ev);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev != null)
        {
            ev.IsDeleted = true;
            _context.Update(ev);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        string userId = this.GetUserId();
        EventDetailsViewModel? model = await _eventService.GetEventDetailsAsync(id,userId);

        return View(model);
    }

}
