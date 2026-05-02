using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{
    public class CategoryRepo
    {
        CatalogsBooksContext _context;

        public CategoryRepo(CatalogsBooksContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByID(int id)
        {
            return await _context.Categories.
            FirstOrDefaultAsync(c => c.CategoryID == id);
        }

        public async Task AddCategory(Category newCategory)
        {
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
        }
        public async Task<bool> ModifyCategory(int id, string newMainCategory, string newSubcategory)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);
            if (category != null)
            {
                category.MainCategory = newMainCategory;
                category.SubCategory = newSubcategory;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<Category> FindCategoryAsync(string mainCategory, string subcategory)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.MainCategory.ToLower() == mainCategory.ToLower() &&
                    c.SubCategory.ToLower() == subcategory.ToLower());
        }
    }
}