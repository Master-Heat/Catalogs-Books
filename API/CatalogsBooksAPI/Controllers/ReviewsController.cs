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
    public class ReviewsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public ReviewsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/reviews
        /// Retrieves all reviews from the database
        /// </summary>
        /// <returns>List of all reviews</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Review>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Review>>> GetAllReviews()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Book)
                    .Include(r => r.Account)
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving reviews", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/reviews
        /// Adds a new review to the database
        /// </summary>
        /// <param name="review">Review details to create</param>
        /// <returns>Created review</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Review), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (review.BookID <= 0)
                {
                    return BadRequest(new { message = "Valid BookID is required" });
                }

                if (review.AccountID <= 0)
                {
                    return BadRequest(new { message = "Valid AccountID is required" });
                }

                if (review.RateValue < 0 || review.RateValue > 5)
                {
                    return BadRequest(new { message = "Rate value must be between 0 and 5" });
                }

                // Verify book and account exist
                var bookExists = await _context.Books.AnyAsync(b => b.BookID == review.BookID);
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == review.AccountID);

                if (!bookExists || !accountExists)
                {
                    return BadRequest(new { message = "Invalid BookID or AccountID" });
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReviewById), new { id = review.ReviewID }, review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating review", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/reviews/{id}
        /// Retrieves a specific review by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Review>> GetReviewById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Account)
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null)
            {
                return NotFound(new { message = $"Review with ID {id} not found" });
            }

            return Ok(review);
        }
    }
}