using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogsBooksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public CategoriesController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Categories
        /// Retrieves all categories from the database
        /// </summary>
        [HttpGet]
        public IActionResult DisplayAllCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        /// <summary>
        /// POST: api/Categories
        /// Adds a new category using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateCategory([FromForm] string mainCategory, [FromForm] string subcategory)
        {
            // Validation for required fields
            if (string.IsNullOrWhiteSpace(mainCategory))
            {
                return BadRequest(new { message = "Main Category is required." });
            }

            var category = new Category
            {
                MainCategory = mainCategory,
                SubCategory = subcategory // Matches the property name 'Sbucategory' in your model
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryID }, category);
        }

        /// <summary>
        /// GET: api/Categories/{id}
        /// Retrieves a specific category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            return Ok(category);
        }
    }
}