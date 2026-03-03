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
    public class BooksController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public BooksController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/books
        /// Retrieves all books from the database
        /// </summary>
        /// <returns>List of all books</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Book>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.BookSeire)
                    .Include(b => b.Account)
                    .ToListAsync();

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving books", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/books
        /// Adds a new book to the database
        /// </summary>
        /// <param name="book">Book details to create</param>
        /// <returns>Created book</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Book), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(book.Title))
                {
                    return BadRequest(new { message = "Title is required" });
                }

                if (string.IsNullOrWhiteSpace(book.AuthorName))
                {
                    return BadRequest(new { message = "Author name is required" });
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBookById), new { id = book.BookID }, book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating book", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/books/{id}
        /// Retrieves a specific book by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Book), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookSeire)
                .Include(b => b.Account)
                .FirstOrDefaultAsync(b => b.BookID == id);

            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            return Ok(book);
        }
    }
}