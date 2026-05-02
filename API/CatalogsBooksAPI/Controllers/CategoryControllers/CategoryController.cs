
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CatalogsBooksAPI.DTOs.CategoryDTOs;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CatalogsBooksAPI.Controllers.CategoryController

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // This covers EVERY function in this controller
    public class CategoryController : ControllerBase
    {
        CategoryFactory _categoryFactory;

        public CategoryController(CategoryFactory categoryFactory)
        {
            _categoryFactory = categoryFactory;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryFactory.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("AllCategoriesWithBooks")]
        public async Task<IActionResult> GetAllCategoriesWithBooks()
        {
            var categories = await _categoryFactory.GetAllCategoriesWithBooksAsync();
            return Ok(categories);
        }
        [HttpGet("{categoryid}")]
        public async Task<IActionResult> GetCategoryByID(int categoryid)
        {
            var category = await _categoryFactory.GetCategoryByIDAsync(categoryid);
            if (category == null) return NotFound($"No category found with ID '{categoryid}'.");

            return Ok(category);
        }
        [HttpGet("CategoryWithBooks/{categoryid}")]
        public async Task<IActionResult> GetCategoryWithBooks(int categoryid)
        {
            var categoryWithBooks = await _categoryFactory.GetCategoryWithBooksAsync(categoryid);
            if (categoryWithBooks == null) return NotFound($"No category found with ID '{categoryid}'.");

            return Ok(categoryWithBooks);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryFactory.CreateCategoryFromDTOAsync(category);
            return Ok(category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyCategory(int id, [FromBody] CategoryCreateDTO updatedCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _categoryFactory.ModifyCategoryAsync(id, updatedCategory.MainCategory, updatedCategory.Subcategory);
            if (!success) return NotFound($"No category found with ID '{id}'.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryFactory.DeleteCategoryAsync(id);
            if (!success) return NotFound($"No category found with ID '{id}'.");

            return NoContent();
        }


    }
}