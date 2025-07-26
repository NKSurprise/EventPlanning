using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Event;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EventPlanningAndManagementSystem.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public EventService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<EventIndexViewModel>> GetAllEventsAsync(string? userId, string? search = null, int? categoryId = null)
        {
            var query = context.Events
                .AsNoTracking()
                .Include(e => e.Location)
                .Include(e => e.Category)
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.PublishedOn)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            return await query.Select(e => new EventIndexViewModel
            {
                Id = e.Id,
                Name = e.Name,
                ImageUrl = e.ImageUrl,
                PublishedOn = e.PublishedOn,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                LocationId = e.LocationId
            }).ToListAsync();
        }

        public async Task<EventDetailsViewModel?> GetEventDetailsAsync(int? id, string? userId)
        {
            if (id == null) return null;

            var e = await context.Events
                .Include(x => x.Category)
                .Include(x => x.Location)
                .Include(x => x.Publisher)
                .Include(x => x.Registrations)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (e == null) return null;

            var isRegistered = await context.Registrations.AnyAsync(r => r.EventId == id && r.UserId == userId);

            return new EventDetailsViewModel
            {
                Id = e.Id,
                Name = e.Name,
                ImageUrl = e.ImageUrl,
                Description = e.Description,
                PublishedOn = e.PublishedOn.ToString("dd-MM-yyyy"),
                CategoryName = e.Category.Name,
                LocationName = e.Location.Name,
                PublisherName = e.Publisher.UserName,
                IsUserRegistered = isRegistered,
                IsConfirmed = e.Registrations.Any(r => r.IsConfirmed),
            };
        }

        public async Task<bool> AddEventAsync(string userId, AddEventInputModel inputModel)
        {
            if (!DateTime.TryParseExact(inputModel.PublishedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                date = DateTime.UtcNow;
            }

            var newEvent = new Event
            {
                Name = inputModel.Name,
                Description = inputModel.Description,
                ImageUrl = inputModel.ImageUrl,
                PublishedOn = date,
                CategoryId = inputModel.CategoryId,
                LocationId = inputModel.LocationId,
                PublisherId = userId,
                IsDeleted = false
            };

            context.Events.Add(newEvent);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<AddEventInputModel> PrepareNewEventFormModelAsync()
        {
            return new AddEventInputModel
            {
                PublishedOn = DateTime.Now.ToString("dd-MM-yyyy"),
                Categories = await GetCategorySelectListAsync(),
                Locations = await GetLocationSelectListAsync()
            };
        }

        public async Task<AddEventInputModel> PrepareNewEventFormModelAsync(AddEventInputModel model)
        {
            model.Categories = await GetCategorySelectListAsync();
            model.Locations = await GetLocationSelectListAsync();
            return model;
        }

        public async Task<EditEventViewModel?> GetEventForEditAsync(string userId, int? id)
        {
            if (id == null) return null;

            var e = await context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(ev => ev.Id == id && ev.PublisherId == userId && !ev.IsDeleted);

            if (e == null) return null;

            return new EditEventViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                ImageUrl = e.ImageUrl,
                CategoryId = e.CategoryId,
                LocationId = e.LocationId,
                PublishedOn = e.PublishedOn,
                Categories = await GetCategorySelectListAsync(e.CategoryId),
                Locations = await GetLocationSelectListAsync(e.LocationId)
            };
        }

        public async Task<EditEventViewModel> PrepareEditEventModelAsync(EditEventViewModel model)
        {
            model.Categories = await GetCategorySelectListAsync(model.CategoryId);
            model.Locations = await GetLocationSelectListAsync(model.LocationId);
            return model;
        }

        public async Task<bool> PersistUpdatedEventAsync(string userId, EditEventViewModel inputModel)
        {
            var e = await context.Events.FirstOrDefaultAsync(ev => ev.Id == inputModel.Id && ev.PublisherId == userId);
            if (e == null) return false;

            e.Name = inputModel.Name;
            e.Description = inputModel.Description;
            e.ImageUrl = inputModel.ImageUrl;
            e.CategoryId = inputModel.CategoryId;
            e.LocationId = inputModel.LocationId;
            e.PublishedOn = inputModel.PublishedOn;

            context.Events.Update(e);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEventAsync(int eventId, string userId)
        {
            var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.PublisherId == userId);
            if (ev == null) return false;

            context.Events.Remove(ev);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<SelectList> GetCategorySelectListAsync(int? selectedId = null)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return new SelectList(categories, "Id", "Name", selectedId);
        }

        private async Task<SelectList> GetLocationSelectListAsync(int? selectedId = null)
        {
            var locations = await context.Locations.AsNoTracking().ToListAsync();
            return new SelectList(locations, "Id", "Name", selectedId);
        }

    }
}


