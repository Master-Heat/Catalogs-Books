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
    public class UserPreferedAuthorsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public UserPreferedAuthorsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/UserPreferedAuthors
        /// Retrieves each row in the table
        /// </summary>
        [HttpGet]
        public IActionResult DisplayAllUserPreferedAuthors()
        {
            var userPreferedAuthors = _context.UserPreferedAuthors.ToList();
            return Ok(userPreferedAuthors);
        }

        /// <summary>
        /// POST: api/UserPreferedAuthors
        /// Adds one row to the DB using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateUserPreferedAuthor([FromForm] int accountId, [FromForm] int authorId)
        {
            // 1. Validate IDs are present and valid
            if (accountId <= 0 || authorId <= 0)
            {
                return BadRequest(new { message = "Both AccountID and AuthorID are required and must be greater than 0." });
            }

            // 2. Verify Account exists
            var accountExists = _context.Accounts.Any(a => a.AccountID == accountId);
            if (!accountExists)
            {
                return BadRequest(new { message = "Invalid AccountID. The specified Account does not exist." });
            }

            // 3. Verify Author exists
            var authorExists = _context.Authors.Any(a => a.AuthorID == authorId);
            if (!authorExists)
            {
                return BadRequest(new { message = "Invalid AuthorID. The specified Author does not exist." });
            }

            // 4. Check for existing preference to prevent duplicates
            var alreadyExists = _context.UserPreferedAuthors
                .Any(upa => upa.AccountID == accountId && upa.AuthorID == authorId);

            if (alreadyExists)
            {
                return BadRequest(new { message = "This author preference already exists for this account." });
            }

            // 5. Map and Save
            var userPreferedAuthor = new UserPreferedAuthor
            {
                AccountID = accountId,
                AuthorID = authorId
            };

            _context.UserPreferedAuthors.Add(userPreferedAuthor);
            _context.SaveChanges();

            return StatusCode(201, userPreferedAuthor);
        }

        /// <summary>
        /// GET: api/UserPreferedAuthors/{accountId}/{authorId}
        /// Retrieves a specific preference by composite ID
        /// </summary>
        [HttpGet("{accountId}/{authorId}")]
        public async Task<ActionResult<UserPreferedAuthor>> GetUserPreferedAuthorById(int accountId, int authorId)
        {
            var preference = await _context.UserPreferedAuthors
                .Include(upa => upa.Account)
                .Include(upa => upa.Author)
                .FirstOrDefaultAsync(upa => upa.AccountID == accountId && upa.AuthorID == authorId);

            if (preference == null)
            {
                return NotFound(new { message = "User preferred author entry not found." });
            }

            return Ok(preference);
        }
    }
}