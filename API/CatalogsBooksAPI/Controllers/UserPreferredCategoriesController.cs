using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogsBooksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferredCategoriesController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public UserPreferredCategoriesController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/userpreferredcategories
        /// Retrieves all user preferred categories from the database
        /// </summary>
        /// <returns>List of all user preferred categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserPreferredCategory>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UserPreferredCategory>>> GetAllUserPreferredCategories()
        {
            try
            {
                var userPreferredCategories = await _context.UserPreferedCategories
                    .Include(upc => upc.Account)
                    .Include(upc => upc.Book)
                    .ToListAsync();

                return Ok(userPreferredCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user preferred categories", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/userpreferredcategories
        /// Adds a user preferred category to the database
        /// </summary>
        /// <param name="userPreferredCategory">User preferred category details</param>
        /// <returns>Created user preferred category</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserPreferredCategory), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserPreferredCategory>> CreateUserPreferredCategory([FromBody] UserPreferredCategory userPreferredCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (userPreferredCategory.AccountID <= 0)
                {
                    return BadRequest(new { message = "Valid AccountID is required" });
                }

                if (string.IsNullOrWhiteSpace(userPreferredCategory.Category))
                {
                    return BadRequest(new { message = "Category is required" });
                }

                if (string.IsNullOrWhiteSpace(userPreferredCategory.SubCategory))
                {
                    return BadRequest(new { message = "SubCategory is required" });
                }

                // Verify account exists
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == userPreferredCategory.AccountID);
                if (!accountExists)
                {
                    return BadRequest(new { message = "Invalid AccountID" });
                }

                _context.UserPreferedCategories.Add(userPreferredCategory);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserPreferredCategoryByIds), 
                    new { accountId = userPreferredCategory.AccountID, category = userPreferredCategory.Category, subCategory = userPreferredCategory.SubCategory }, 
                    userPreferredCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user preferred category", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/userpreferredcategories/{accountId}/{category}/{subCategory}
        /// Retrieves a specific user preferred category
        /// </summary>
        [HttpGet("{accountId}/{category}/{subCategory}")]
        [ProducesResponseType(typeof(UserPreferredCategory), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserPreferredCategory>> GetUserPreferredCategoryByIds(int accountId, string category, string subCategory)
        {
            var userPreferredCategory = await _context.UserPreferedCategories
                .Include(upc => upc.Account)
                .Include(upc => upc.Book)
                .FirstOrDefaultAsync(upc => upc.AccountID == accountId && upc.Category == category && upc.SubCategory == subCategory);

            if (userPreferredCategory == null)
            {
                return NotFound(new { message = $"Preferred category record not found" });
            }

            return Ok(userPreferredCategory);
        }
    }
}