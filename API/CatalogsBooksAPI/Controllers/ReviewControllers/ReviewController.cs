using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace CatalogsBooksAPI.Controllers.ReviewController

{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {

        ReviewFactory _reviewFactory;
        public ReviewsController(ReviewFactory reviewFactory)
        {
            _reviewFactory = reviewFactory;
        }
        protected int GetUserId()
        {
            var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(id, out int result) ? result : 0;
        }


        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> SubmitReview(int BookID, string ReviewText, double RateValue)
        {
            int IdFromToken = GetUserId();
            if (IdFromToken == 0)
            { return Unauthorized(); }

            try
            {
                // Call the factory logic
                await _reviewFactory.CreateOrUpdateReviewAsync(
                    IdFromToken,
                    BookID,
                    ReviewText,
                   RateValue
                );

                return Ok(new { message = "Review submitted successfully." });
            }
            catch (ArgumentException ex)
            {
                // Catches the validation errors from your ValidateReviewDTO method
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // General error handling
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}