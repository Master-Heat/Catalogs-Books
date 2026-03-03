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
    public class UserPreferedAuthorsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public UserPreferedAuthorsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/userpreferedauthors
        /// Retrieves all user preferred authors from the database
        /// </summary>
        /// <returns>List of all user preferred authors</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserPreferedAuthor>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UserPreferedAuthor>>> GetAllUserPreferedAuthors()
        {
            try
            {
                var userPreferedAuthors = await _context.UserPreferedAuthors
                    .Include(upa => upa.Account)
                    .Include(upa => upa.Book)
                    .ToListAsync();

                return Ok(userPreferedAuthors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user preferred authors", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/userpreferedauthors
        /// Adds a user preferred author to the database
        /// </summary>
        /// <param name="userPreferedAuthor">User preferred author details</param>
        /// <returns>Created user preferred author</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserPreferedAuthor), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserPreferedAuthor>> CreateUserPreferedAuthor([FromBody] UserPreferedAuthor userPreferedAuthor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (userPreferedAuthor.AccountID <= 0)
                {
                    return BadRequest(new { message = "Valid AccountID is required" });
                }

                if (string.IsNullOrWhiteSpace(userPreferedAuthor.AuthorName))
                {
                    return BadRequest(new { message = "Author name is required" });
                }

                // Verify account exists
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == userPreferedAuthor.AccountID);
                if (!accountExists)
                {
                    return BadRequest(new { message = "Invalid AccountID" });
                }

                _context.UserPreferedAuthors.Add(userPreferedAuthor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserPreferedAuthorByIds), 
                    new { accountId = userPreferedAuthor.AccountID, authorName = userPreferedAuthor.AuthorName }, 
                    userPreferedAuthor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user preferred author", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/userpreferedauthors/{accountId}/{authorName}
        /// Retrieves a specific user preferred author by AccountID and AuthorName
        /// </summary>
        [HttpGet("{accountId}/{authorName}")]
        [ProducesResponseType(typeof(UserPreferedAuthor), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserPreferedAuthor>> GetUserPreferedAuthorByIds(int accountId, string authorName)
        {
            var userPreferedAuthor = await _context.UserPreferedAuthors
                .Include(upa => upa.Account)
                .Include(upa => upa.Book)
                .FirstOrDefaultAsync(upa => upa.AccountID == accountId && upa.AuthorName == authorName);

            if (userPreferedAuthor == null)
            {
                return NotFound(new { message = $"Preferred author record not found for AccountID {accountId} and Author {authorName}" });
            }

            return Ok(userPreferedAuthor);
        }
    }
}