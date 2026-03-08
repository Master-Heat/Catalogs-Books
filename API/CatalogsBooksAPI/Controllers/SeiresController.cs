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
    public class SeiresController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public SeiresController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/SeiresController
        /// Retrieves all book series from the database
        /// </summary>
        /// <returns>List of all book series</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Seire>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Seire>>> GetAllSeires()
        {
            try
            {
                var Seires = await _context.Seires
                    .Include(bs => bs.Books)
                    .ToListAsync();

                return Ok(Seires);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving book series", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/SeiresController
        /// Adds a new book series to the database
        /// </summary>
        /// <param name="Seire">Book series details to create</param>
        /// <returns>Created book series</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Seire), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Seire>> CreateSeire([FromBody] Seire Seire)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(Seire.SeireName))
                {
                    return BadRequest(new { message = "Series name is required" });
                }

                _context.Seires.Add(Seire);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSeireById), new { id = Seire.BookID }, Seire);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating book series", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/SeiresController/{id}
        /// Retrieves a specific book series by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Seire), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Seire>> GetSeireById(int id)
        {
            var Seire = await _context.Seires
                .Include(bs => bs.Books)
                .FirstOrDefaultAsync(bs => bs.BookID == id);

            if (Seire == null)
            {
                return NotFound(new { message = $"Book series with ID {id} not found" });
            }

            return Ok(Seire);
        }
    }
}