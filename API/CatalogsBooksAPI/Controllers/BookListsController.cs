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
    public class BookListsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public BookListsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/BookLists
        /// Retrieves all book list entries from the database
        /// </summary>
        [HttpGet]
        public IActionResult DisplayAllBookListItems()
        {
            var bookListItems = _context.BookLists.ToList();
            return Ok(bookListItems);
        }

        /// <summary>
        /// POST: api/BookLists
        /// Adds a book to a user list using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateBookListItem([FromForm] int listId, [FromForm] int bookId)
        {
            // 1. Validate IDs are present and valid
            if (listId <= 0 || bookId <= 0)
            {
                return BadRequest(new { message = "Both ListID and BookID are required and must be greater than 0." });
            }

            // 2. Verify UserList exists
            var listExists = _context.UserLists.Any(ul => ul.ListID == listId);
            if (!listExists)
            {
                return BadRequest(new { message = "Invalid ListID. The specified User List does not exist." });
            }

            // 3. Verify Book exists
            var bookExists = _context.Books.Any(b => b.BookID == bookId);
            if (!bookExists)
            {
                return BadRequest(new { message = "Invalid BookID. The specified Book does not exist." });
            }

            // 4. Check for existing entry to prevent duplicates in the same list
            var alreadyExists = _context.BookLists
                .Any(bl => bl.ListID == listId && bl.BookID == bookId);

            if (alreadyExists)
            {
                return BadRequest(new { message = "This book is already in the specified list." });
            }

            // 5. Map and Save
            var bookListItem = new BookList
            {
                ListID = listId,
                BookID = bookId
            };

            _context.BookLists.Add(bookListItem);
            _context.SaveChanges();

            return StatusCode(201, bookListItem);
        }

        /// <summary>
        /// GET: api/BookLists/{listId}/{bookId}
        /// Retrieves a specific book list entry by composite ID
        /// </summary>
        [HttpGet("{listId}/{bookId}")]
        public async Task<ActionResult<BookList>> GetBookListItemById(int listId, int bookId)
        {
            var bookListItem = await _context.BookLists
                .Include(bl => bl.UserList)
                .Include(bl => bl.Book)
                .FirstOrDefaultAsync(bl => bl.ListID == listId && bl.BookID == bookId);

            if (bookListItem == null)
            {
                return NotFound(new { message = "Book list entry not found." });
            }

            return Ok(bookListItem);
        }
    }
}