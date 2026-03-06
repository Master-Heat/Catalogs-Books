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
    public class ViewedBooksController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public ViewedBooksController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/viewedbooks
        /// Retrieves all viewed books from the database
        /// </summary>
        /// <returns>List of all viewed books</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ViewedBook>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ViewedBook>>> GetAllViewedBooks()
        {
            try
            {
                var viewedBooks = await _context.ViewedBooks
                    .Include(vb => vb.Book)
                    .Include(vb => vb.Account)
                    .ToListAsync();

                return Ok(viewedBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving viewed books", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/viewedbooks
        /// Records a viewed book in the database
        /// </summary>
        /// <param name="viewedBook">Viewed book details</param>
        /// <returns>Created viewed book record</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ViewedBook), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ViewedBook>> CreateViewedBook([FromBody] ViewedBook viewedBook)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (viewedBook.AccountID <= 0)
                {
                    return BadRequest(new { message = "Valid AccountID is required" });
                }

                if (viewedBook.BookID <= 0)
                {
                    return BadRequest(new { message = "Valid BookID is required" });
                }

                // Verify account and book exist
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == viewedBook.AccountID);
                var bookExists = await _context.Books.AnyAsync(b => b.BookID == viewedBook.BookID);

                if (!accountExists || !bookExists)
                {
                    return BadRequest(new { message = "Invalid AccountID or BookID" });
                }

                _context.ViewedBooks.Add(viewedBook);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetViewedBookByIds), new { accountId = viewedBook.AccountID, bookId = viewedBook.BookID }, viewedBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating viewed book record", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/viewedbooks/{accountId}/{bookId}
        /// Retrieves a specific viewed book record by AccountID and BookID
        /// </summary>
        [HttpGet("{accountId}/{bookId}")]
        [ProducesResponseType(typeof(ViewedBook), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ViewedBook>> GetViewedBookByIds(int accountId, int bookId)
        {
            var viewedBook = await _context.ViewedBooks
                .Include(vb => vb.Book)
                .Include(vb => vb.Account)
                .FirstOrDefaultAsync(vb => vb.AccountID == accountId && vb.BookID == bookId);

            if (viewedBook == null)
            {
                return NotFound(new { message = $"Viewed book record not found for AccountID {accountId} and BookID {bookId}" });
            }

            return Ok(viewedBook);
        }
    }
}