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
        [HttpGet]
        public IActionResult DisplayAllBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        /// <summary>
        /// POST: api/books
        /// Adds a new book to the database using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateBook(
            [FromForm] int authorId,
            [FromForm] string title,

            [FromForm] int? seriesId,
            [FromForm] DateOnly publicationDate,
            [FromForm] bool canDownload,
            [FromForm] string downloadLink,
            [FromForm] string description,
            [FromForm] int categoryId,
            [FromForm] string coverImageLink,
            [FromForm] string coverAlt,
            [FromForm] int pagesCount)
        {
            // Validation for required fields based on the Book model
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { message = "Title is required" });
            }


            var book = new Book
            {
                AuthorID = authorId,
                Title = title,

                SeireID = seriesId,
                PublicationDate = publicationDate,
                CanDownload = canDownload,
                DownloadLink = downloadLink,
                Description = description,
                CategoryID = categoryId,
                CoverImageLink = coverImageLink,
                CoverAlt = coverAlt,
                PagesCount = pagesCount
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBookById), new { id = book.BookID }, book);
        }

        /// <summary>
        /// GET: api/books/{id}
        /// Retrieves a specific book by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookSeire)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookID == id);

            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            return Ok(book);
        }
    }
}