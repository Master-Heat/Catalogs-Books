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
            [FromForm] DateOnly? publicationDate, // Changed to nullable DateOnly?
            [FromForm] bool canDownload,
            [FromForm] string downloadLink,
            [FromForm] string description,
            [FromForm] int categoryId,
            [FromForm] string coverImageLink,
            [FromForm] string coverAlt,
            [FromForm] int pagesCount)
        {
            // 1. Validation for required fields
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { message = "Title is required" });
            }




            // Note: If publicationDate is sent as an empty string ("") 
            // ASP.NET Core will automatically bind it as null because of the DateOnly? type.

            // 3. Verify Foreign Keys exist if they are provided
            if (authorId > 0)
            {
                var authorExists = _context.Authors.Any(a => a.AuthorID == authorId);
                if (!authorExists) return BadRequest(new { message = "Invalid AuthorID." });
            }

            if (categoryId > 0)
            {
                var categoryExists = _context.Categories.Any(c => c.CategoryID == categoryId);
                if (!categoryExists) return BadRequest(new { message = "Invalid CategoryID." });
            }

            // 4. Map and Save
            var book = new Book
            {
                AuthorID = authorId,
                Title = title,

                PublicationDate = (DateOnly)(publicationDate.HasValue ? publicationDate.Value : default(DateOnly)),
                // Note: If your database column is NOT NULL, it needs a default. 
                // If the DB column IS NULL, change the Book.cs model property to DateOnly? 
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
                .Include(b => b.Series)
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