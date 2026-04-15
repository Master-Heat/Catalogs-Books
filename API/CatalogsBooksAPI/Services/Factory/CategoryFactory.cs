namespace CatalogsBooksAPI.Services.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using CatalogsBooksAPI.DTOs.CategoryDTOs;
    using CatalogsBooksAPI.Models;
    using Microsoft.EntityFrameworkCore;

    public interface ICategoryFactory
    {
        Task<Category> FindCategoryAsync(string mainCategory, string subcategory);
        Task<Category> CreateCategoryFromDTOAsync(CategoryInfoDTO categoryDto);
    }




    public class CategoryFactory : ICategoryFactory
    {
        private readonly CatalogsBooksContext _context;

        public CategoryFactory(CatalogsBooksContext context)
        {
            _context = context;
        }

        // 1. Search Logic: Checks both fields to find an exact pair match
        public async Task<Category> FindCategoryAsync(string mainCategory, string subcategory)
        {
            // We search for a row where both match (case-insensitive)
            return await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.MainCategory.ToLower() == mainCategory.ToLower() &&
                    c.Sbucategory.ToLower() == subcategory.ToLower());
        }

        // 2. Creation Logic
        public async Task<Category> CreateCategoryFromDTOAsync(CategoryInfoDTO categoryDto)
        {
            // First, validate the input
            ValidateCategoryDTO(categoryDto);

            // Second, check if this specific Main/Sub pair already exists
            var existingCategory = await FindCategoryAsync(categoryDto.MainCategory, categoryDto.Sbucategory);

            if (existingCategory != null)
            {
                return existingCategory;
            }

            // Third, map to the model
            var newCategory = new Category
            {
                MainCategory = categoryDto.MainCategory,
                Sbucategory = categoryDto.Sbucategory
            };

            _context.Categories.Add(newCategory);

            return newCategory;
        }

        private void ValidateCategoryDTO(CategoryInfoDTO categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto), "Category data cannot be null.");

            if (string.IsNullOrWhiteSpace(categoryDto.MainCategory))
                throw new ArgumentException("Main Category is required.");

            if (string.IsNullOrWhiteSpace(categoryDto.Sbucategory))
                throw new ArgumentException("Subcategory is required.");
        }
    }
}





