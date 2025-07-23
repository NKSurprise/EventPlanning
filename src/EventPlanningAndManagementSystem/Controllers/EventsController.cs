using EventPlanningAndManagementSystem.Controllers;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.ViewModels.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using X.PagedList.Extensions;

public class EventsController : BaseController
{
    private readonly IEventService _eventService;
    private readonly UserManager<IdentityUser> _userManager;

    public EventsController(IEventService eventService, UserManager<IdentityUser> userManager)
    {
        _eventService = eventService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? page)
    {
        var userId = _userManager.GetUserId(User);
        var events = await _eventService.GetAllEventsAsync(userId, searchTerm, categoryId);

        ViewData["CurrentFilter"] = searchTerm;
        ViewData["CurrentCategory"] = categoryId;
        ViewBag.Categories = await _eventService.GetCategorySelectListAsync();

        int pageSize = 8;
        int pageNumber = page ?? 1;
        var pagedEvents = events.ToPagedList(pageNumber, pageSize);

        return View(pagedEvents);
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create()
    {
        var model = await _eventService.PrepareNewEventFormModelAsync();
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(AddEventInputModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await _eventService.PrepareNewEventFormModelAsync(model);
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        await _eventService.AddEventAsync(userId, model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var model = await _eventService.GetEventForEditAsync(userId, id);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id, EditEventViewModel model)
    {
        if (id != model.Id || !ModelState.IsValid)
        {
            model = await _eventService.PrepareEditEventModelAsync(model);
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        var success = await _eventService.PersistUpdatedEventAsync(userId, model);
        if (!success)
        {
            ModelState.AddModelError("", "Error saving changes.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        await _eventService.DeleteEventAsync(id, userId);
        return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var model = await _eventService.GetEventDetailsAsync(id, userId);
        return View(model);
    }
}
