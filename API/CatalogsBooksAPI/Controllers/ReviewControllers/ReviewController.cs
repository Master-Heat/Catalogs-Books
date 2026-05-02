using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Controllers.ReviewController

{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {

        ReviewFactory _reviewFactory;
        RateAndReviewRepo _reviewRepo;
        public ReviewsController(ReviewFactory reviewFactory, RateAndReviewRepo rateAndReviewRepo)
        {
            _reviewFactory = reviewFactory;
            _reviewRepo = rateAndReviewRepo;
        }
        [NonAction]
        protected int GetUserId()
        {
            var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(id, out int result) ? result : 0;
        }
        [NonAction]
        protected string GetAccountRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                   ?? User.FindFirst("role")?.Value
                   ?? string.Empty;
        }



        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> SubmitReview(int BookID, string ReviewText, double? RateValue)
        {
            int IdFromToken = GetUserId();
            if (IdFromToken == 0)
            { return Unauthorized(); }

            if (RateValue == null)
            {
                return BadRequest("Rate must have a value");
            }
            double rate = (double)RateValue;


            try
            {
                // Call the factory logic
                await _reviewFactory.CreateOrUpdateReviewAsync(
                    IdFromToken,
                    BookID,
                    ReviewText,
                    rate
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

        [Authorize]
        [HttpGet("user/{accountid:int}")]
        public async Task<IActionResult> GetUserReviews(int accountid)
        {
            string role = GetAccountRole();
            if (string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized();
            }
            if (role == "Admin" || role == "AI")
            {
                List<UserReviewDTO> reviews = await _reviewRepo.GetUserReviews(accountid);
                if (reviews == null || reviews.Count == 0) return NotFound();
                return Ok(reviews);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("user/")]
        public async Task<IActionResult> GetMyReviews()
        {
            int accountid = GetUserId();

            if (accountid == 0)
            {
                return Unauthorized();
            }

            List<UserReviewDTO> reviews = await _reviewRepo.GetUserReviews(accountid);
            if (reviews == null || reviews.Count == 0) return NotFound();
            return Ok(reviews);

        }
        [Authorize]
        [HttpGet("book/{bookid}")]
        public async Task<IActionResult> GetAllBookReviews(int bookid)
        {
            string role = GetAccountRole();
            if (string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized();
            }
            if (role == "Admin" || role == "AI")
            {
                List<RateAndReviewDTO> reviews = await _reviewFactory.GetAllBookReviews(bookid);
                if (reviews == null || reviews.Count == 0) return NotFound();
                return Ok(reviews);
            }
            return BadRequest();
        }
    }
}
