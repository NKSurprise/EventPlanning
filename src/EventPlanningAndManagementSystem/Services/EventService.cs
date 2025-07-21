using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Event;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EventPlanningAndManagementSystem.EventPlanningAndManagementSystem.GCommon.ValidationConstatnts.Event;

namespace EventPlanningAndManagementSystem.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IEventService"/> that talks directly to the EF Core
    /// <see cref="ApplicationDbContext"/>.  The service hides all data‑access details from the controllers
    /// and exposes simple async methods that return strongly‑typed view‑models ready for the UI layer.
    /// </summary>
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        #region Queries

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
            {
                query = query.Where(e => e.Name.Contains(search));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var events = await query
                .Select(e => new EventIndexViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    ImageUrl = e.ImageUrl,
                    PublishedOn = e.PublishedOn,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name,
                    LocationId = e.LocationId
                })
                .ToListAsync();

            return events;
        }


        public async Task<EventDetailsViewModel?> GetEventDetailsAsync(int? id, string userId)
        {
            if (id == null)
                return null;

            Event? eventModel = await this.context
                .Events
                .Include(ev => ev.Location)
                .Include(ev => ev.Category)
                .Include(ev => ev.Publisher)
                .Include(ev => ev.Registrations)
                .AsNoTracking()
                .SingleOrDefaultAsync(ev => ev.Id == id.Value);


            if (eventModel == null)
                return null;
            var isRegistered = await context.Registrations
            .AnyAsync(r => r.EventId == id && r.UserId == userId);

            return new EventDetailsViewModel
            {
                Id = eventModel.Id,
                Name = eventModel.Name,
                ImageUrl = eventModel.ImageUrl,
                Description = eventModel.Description,
                PublishedOn = eventModel.PublishedOn.ToString(PublishedOnFormat),
                CategoryName = eventModel.Category.Name,
                LocationName = eventModel.Location.Name,
                PublisherName = eventModel.Publisher.UserName,
                IsUserRegistered = isRegistered
            };

        }

        #endregion

        #region Commands

        public async Task<bool> AddEventAsync(string userId, AddEventInputModel inputModel)
        {
            if (inputModel == null)
            {
                return false;
            }

            if (!DateTime.TryParseExact(inputModel.PublishedOn, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var publishedOn))
            {
                publishedOn = DateTime.Now;
            }

            var entity = new Event
            {
                Name = inputModel.Name,
                Description = inputModel.Description,
                ImageUrl = inputModel.ImageUrl,
                PublishedOn = publishedOn,
                CategoryId = inputModel.CategoryId,
                LocationId = inputModel.LocationId,
                PublisherId = userId,
                IsDeleted = false
            };

            context.Events.Add(entity);
            var saved = await context.SaveChangesAsync();
            inputModel.Id = entity.Id;
            return saved > 0;
        }

        public async Task<EditEventViewModel?> GetEventForEditAsync(string userId, int? id)
        {
            if (id == null)
            {
                return null;
            }

            var e = await context.Events
                .Include(ev => ev.Location)
                .Include(ev => ev.Category)
                .FirstOrDefaultAsync(ev => ev.Id == id.Value && ev.PublisherId == userId);

            if (e == null)
            {
                return null;
            }

            return new EditEventViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                ImageUrl = e.ImageUrl,
                PublishedOn = e.PublishedOn,
                CategoryId = e.CategoryId,
                LocationId = e.LocationId,
                Categories = new SelectList(await context.Categories.AsNoTracking().ToListAsync(), "Id", "Name", e.CategoryId),
                Locations = new SelectList(await context.Locations.AsNoTracking().ToListAsync(), "Id", "Name", e.LocationId)
            };
        }

        public async Task<bool> PersistUpdatedEventAsync(string userId, EditEventViewModel inputModel)
        {
            var e = await context.Events.FirstOrDefaultAsync(ev => ev.Id == inputModel.Id && ev.PublisherId == userId);
            if (e == null)
            {
                return false;
            }

            e.Name = inputModel.Name;
            e.Description = inputModel.Description;
            e.ImageUrl = inputModel.ImageUrl;
            e.PublishedOn = inputModel.PublishedOn;
            e.CategoryId = inputModel.CategoryId;
            e.LocationId = inputModel.LocationId;

            context.Events.Update(e);
            return await context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
