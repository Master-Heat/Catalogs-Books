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
    public class AuthorsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public AuthorsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Authors
        /// Retrieves each row in the table
        /// </summary>
        [HttpGet]
        public IActionResult DisplayAllAuthors()
        {
            var authors = _context.Authors.ToList();
            return Ok(authors);
        }

        /// <summary>
        /// POST: api/Authors
        /// Adds one row to the DB using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateAuthor([FromForm] string authorName, [FromForm] int? accountId)
        {
            // 1. Validate required fields
            if (string.IsNullOrWhiteSpace(authorName))
            {
                return BadRequest(new { message = "Author name is required." });
            }

            // 2. Verify Account exists if an AccountID is provided
            if (accountId.HasValue && accountId > 0)
            {
                var accountExists = _context.Accounts.Any(a => a.AccountID == accountId.Value);
                if (!accountExists)
                {
                    return BadRequest(new { message = "Invalid AccountID. The specified Account does not exist." });
                }
            }

            // 3. Map and Save
            var author = new Author
            {
                AuthorName = authorName,
                AccountID = accountId
            };

            _context.Authors.Add(author);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAuthorById), new { id = author.AuthorID }, author);
        }

        /// <summary>
        /// GET: api/Authors/{id}
        /// Retrieves a specific author by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Account)
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.AuthorID == id);

            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found." });
            }

            return Ok(author);
        }
    }
}