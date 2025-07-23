using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data;
using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Category;
using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.ToListAsync();

    public async Task<bool> AddCategoryAsync(AddCategoryViewModel model)
    {
        bool exists = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());

        if (exists) return false;

        var category = new Category { Name = model.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        var softDeletedEvents = await _context.Events
            .Where(e => e.CategoryId == id && e.IsDeleted)
            .ToListAsync();

        _context.Events.RemoveRange(softDeletedEvents);

        try
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
