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
        [HttpGet]
        public IActionResult DisplayAllUserPreferences()
        {
            var preferences = _context.UserPreferedCategories.ToList();
            return Ok(preferences);
        }

        /// <summary>
        /// POST: api/userpreferredcategories
        /// Adds a new user category preference using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateUserPreferredCategory([FromForm] int accountId, [FromForm] int categoryId)
        {
            // 1. Validate IDs are present and valid
            if (accountId <= 0 || categoryId <= 0)
            {
                return BadRequest(new { message = "Both AccountID and CategoryID are required and must be greater than 0." });
            }

            // 2. Verify Account exists
            var accountExists = _context.Accounts.Any(a => a.AccountID == accountId);
            if (!accountExists)
            {
                return BadRequest(new { message = "Invalid AccountID. Account does not exist." });
            }

            // 3. Verify Category exists
            var categoryExists = _context.Categories.Any(c => c.CategoryID == categoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Invalid CategoryID. Category does not exist." });
            }

            // 4. Check for existing preference to prevent duplicates
            var alreadyExists = _context.UserPreferedCategories
                .Any(upc => upc.AccountID == accountId && upc.CategoryID == categoryId);

            if (alreadyExists)
            {
                return BadRequest(new { message = "This preference already exists for this account." });
            }

            // 5. Map and Save
            var userPreferredCategory = new UserPreferredCategory
            {
                AccountID = accountId,
                CategoryID = categoryId
            };

            _context.UserPreferedCategories.Add(userPreferredCategory);
            _context.SaveChanges();

            return StatusCode(201, userPreferredCategory);
        }

        /// <summary>
        /// GET: api/userpreferredcategories/{accountId}/{categoryId}
        /// Retrieves a specific preference by composite ID
        /// </summary>
        [HttpGet("{accountId}/{categoryId}")]
        public async Task<ActionResult<UserPreferredCategory>> GetPreferenceById(int accountId, int categoryId)
        {
            var preference = await _context.UserPreferedCategories
                .Include(upc => upc.Account)
                .Include(upc => upc.Category)
                .FirstOrDefaultAsync(upc => upc.AccountID == accountId && upc.CategoryID == categoryId);

            if (preference == null)
            {
                return NotFound(new { message = "Preference not found." });
            }

            return Ok(preference);
        }
    }
}