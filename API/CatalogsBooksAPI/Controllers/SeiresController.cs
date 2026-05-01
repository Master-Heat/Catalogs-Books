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
    public class SeriesController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public SeriesController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/SeriesController
        /// Retrieves all book series from the database
        /// </summary>
        /// <returns>List of all book series</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Series>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Series>>> GetAllSeries()
        {
            try
            {
                var Series = await _context.Series
                    .Include(bs => bs.Books)
                    .ToListAsync();

                return Ok(Series);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving book series", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/SeriesController
        /// Adds a new book series to the database
        /// </summary>
        /// <param name="Series">Book series details to create</param>
        /// <returns>Created book series</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Series), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Series>> CreateSeries([FromBody] Series Series)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(Series.SeriesName))
                {
                    return BadRequest(new { message = "Series name is required" });
                }

                _context.Series.Add(Series);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSeriesById), new { id = Series.BookID }, Series);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating book series", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/SeriesController/{id}
        /// Retrieves a specific book series by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Series), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Series>> GetSeriesById(int id)
        {
            var Series = await _context.Series
                .Include(bs => bs.Books)
                .FirstOrDefaultAsync(bs => bs.BookID == id);

            if (Series == null)
            {
                return NotFound(new { message = $"Book series with ID {id} not found" });
            }

            return Ok(Series);
        }
    }
}