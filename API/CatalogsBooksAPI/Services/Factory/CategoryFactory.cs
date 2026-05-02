namespace CatalogsBooksAPI.Services.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using CatalogsBooksAPI.DTOs.BooksDTOs;
    using CatalogsBooksAPI.DTOs.CategoryDTOs;
    using CatalogsBooksAPI.Models;
    using CatalogsBooksAPI.Repository;
    using Microsoft.EntityFrameworkCore;
    public class CategoryFactory
    {
        private readonly CategoryRepo _categoryRepo;

        public CategoryFactory(CategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // 1. Search Logic: Checks both fields to find an exact pair match
        public async Task<Category> FindCategoryAsync(string mainCategory, string subcategory)
        {
            // We search for a row where both match (case-insensitive)
            return await _categoryRepo.FindCategoryAsync(mainCategory, subcategory);
        }

        // 2. Creation Logic
        public async Task<Category> CreateCategoryFromDTOAsync(CategoryCreateDTO categoryDto)
        {
            // First, validate the input
            ValidateCategoryDTO(categoryDto);

            // Second, check if this specific Main/Sub pair already exists
            var existingCategory = await FindCategoryAsync(categoryDto.MainCategory, categoryDto.Subcategory);

            if (existingCategory != null)
            {
                return existingCategory;
            }

            // Third, map to the model
            var newCategory = new Category
            {
                MainCategory = categoryDto.MainCategory,
                SubCategory = categoryDto.Subcategory
            };

            await _categoryRepo.AddCategory(newCategory);

            return newCategory;
        }

        private void ValidateCategoryDTO(CategoryCreateDTO categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto), "Category data cannot be null.");

            if (string.IsNullOrWhiteSpace(categoryDto.MainCategory))
                throw new ArgumentException("Main Category is required.");

            if (string.IsNullOrWhiteSpace(categoryDto.Subcategory))
                throw new ArgumentException("Subcategory is required.");
        }
        public async Task<bool> ModifyCategoryAsync(int id, string newMainCategory, string newSubcategory)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(newMainCategory))
                throw new ArgumentException("Main Category is required.");

            if (string.IsNullOrWhiteSpace(newSubcategory))
                throw new ArgumentException("Subcategory is required.");

            // Check if the category exists
            var category = await _categoryRepo.GetCategoryByID(id);
            if (category == null)
                return false;

            // Check for duplicate Main/Sub pair (excluding the current category)
            var duplicateCategory = await _categoryRepo.FindCategoryAsync(newMainCategory, newSubcategory);
            if (duplicateCategory != null && duplicateCategory.CategoryID != id)
                throw new InvalidOperationException("Another category with the same Main and Sub already exists.");

            // Update the category
            return await _categoryRepo.ModifyCategory(id, newMainCategory, newSubcategory);
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Check if the category exists
            var category = await _categoryRepo.GetCategoryByID(id);
            if (category == null)
                return false;

            // Delete the category
            return await _categoryRepo.DeleteCategory(id);
        }
        public async Task<List<CategoryInfoDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllCategories();
            return categories.Select(c => new CategoryInfoDTO
            {
                CategoryID = c.CategoryID,
                MainCategory = c.MainCategory,
                Subcategory = c.SubCategory
            }).ToList();
        }

        public async Task<CategoryInfoDTO> GetCategoryByIDAsync(int id)
        {
            var category = await _categoryRepo.GetCategoryByID(id);
            if (category == null)
                return null;

            return new CategoryInfoDTO
            {
                CategoryID = category.CategoryID,
                MainCategory = category.MainCategory,
                Subcategory = category.SubCategory
            };
        }
        public async Task<CategoryBookCardsDTO> GetCategoryWithBooksAsync(int id)
        {
            var category = await _categoryRepo.GetCategoryByID(id);
            if (category == null)
                return null;

            return new CategoryBookCardsDTO
            {
                CategoryID = category.CategoryID,
                MainCategory = category.MainCategory,
                Subcategory = category.SubCategory,
                BookCards = category.Books.Select(b => new BookCardDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Description = b.Description,
                    CoverImageLink = b.CoverImageLink,
                    CoverAlt = b.CoverAlt

                }).ToList()
            };
        }
        public async Task<List<CategoryBookCardsDTO>> GetAllCategoriesWithBooksAsync()
        {
            var categories = await _categoryRepo.GetAllCategories();
            return categories.Select(c => new CategoryBookCardsDTO
            {
                CategoryID = c.CategoryID,
                MainCategory = c.MainCategory,
                Subcategory = c.SubCategory,
                BookCards = c.Books.Select(b => new BookCardDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Description = b.Description,
                    CoverImageLink = b.CoverImageLink,
                    CoverAlt = b.CoverAlt

                }).ToList()
            }).ToList();
        }
    }

}



