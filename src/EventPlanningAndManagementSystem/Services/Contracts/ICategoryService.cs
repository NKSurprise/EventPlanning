using EventPlanningAndManagementSystem.Data.EventPlanningAndManagementSystem.Data.Models;
using EventPlanningAndManagementSystem.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<bool> AddCategoryAsync(AddCategoryViewModel model);
    Task<Category?> GetByIdAsync(int id);
    Task<bool> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
}
