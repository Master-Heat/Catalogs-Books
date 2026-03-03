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
    public class BookSeiresController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public BookSeiresController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/bookseiresController
        /// Retrieves all book series from the database
        /// </summary>
        /// <returns>List of all book series</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BookSeire>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<BookSeire>>> GetAllBookSeires()
        {
            try
            {
                var bookSeires = await _context.BookSeires
                    .Include(bs => bs.Books)
                    .ToListAsync();

                return Ok(bookSeires);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving book series", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/bookseiresController
        /// Adds a new book series to the database
        /// </summary>
        /// <param name="bookSeire">Book series details to create</param>
        /// <returns>Created book series</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BookSeire), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BookSeire>> CreateBookSeire([FromBody] BookSeire bookSeire)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(bookSeire.SeireName))
                {
                    return BadRequest(new { message = "Series name is required" });
                }

                _context.BookSeires.Add(bookSeire);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBookSeireById), new { id = bookSeire.SeireID }, bookSeire);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating book series", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/bookseiresController/{id}
        /// Retrieves a specific book series by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookSeire), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BookSeire>> GetBookSeireById(int id)
        {
            var bookSeire = await _context.BookSeires
                .Include(bs => bs.Books)
                .FirstOrDefaultAsync(bs => bs.SeireID == id);

            if (bookSeire == null)
            {
                return NotFound(new { message = $"Book series with ID {id} not found" });
            }

            return Ok(bookSeire);
        }
    }
}