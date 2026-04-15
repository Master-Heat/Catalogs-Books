namespace CatalogsBooksAPI.Services.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
    using CatalogsBooksAPI.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Update.Internal;

    public interface IReviewFactory
    {
        Task<Review> CreateOrUpdateReviewAsync(AddRateAndReviewDTO dto);
    }



    public class ReviewFactory : IReviewFactory
    {
        private readonly CatalogsBooksContext _context;

        public ReviewFactory(CatalogsBooksContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateOrUpdateReviewAsync(AddRateAndReviewDTO dto)
        {
            // 1. Validation Logic
            ValidateReviewDTO(dto);

            // 2. Search Logic: Check if this user already reviewed this book
            var existingReview = CheckIfUserReviewedThisBook(dto);

            if (existingReview != null)
            {

                // Logic: Update the existing review instead of creating a new one
                existingReview.RateValue = dto.RateValue;
                existingReview.ReviewText = dto.ReviewText;
                existingReview.ReviewDate = DateTime.Now; // Update the date to 'now'

                // We don't need to call _context.Update because EF tracks the object automatically
                return updateExistingReview(dto, existingReview);
            }

            // 3. Creation Logic: If not found, create a new record
            var newReview = new Review
            {
                BookID = dto.BookID,
                AccountID = dto.AccountID,
                RateValue = dto.RateValue,
                ReviewText = dto.ReviewText,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(newReview);

            return newReview;
        }

        private void ValidateReviewDTO(AddRateAndReviewDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Review data cannot be null.");

            if (dto.BookID <= 0 || dto.AccountID <= 0)
                throw new ArgumentException("Valid BookID and AccountID are required.");

            // Ensure the rate is within a realistic scale (e.g., 1 to 5)
            if (dto.RateValue < 0 || dto.RateValue > 5)
                throw new ArgumentException("Rating must be between 0 and 5.");
        }
        public Review CheckIfUserReviewedThisBook(AddRateAndReviewDTO dto)
        {
            return _context.Reviews
                .AsQueryable()
                .FirstOrDefault(r => r.BookID == dto.BookID && r.AccountID == dto.AccountID);
        }
        public Review updateExistingReview(AddRateAndReviewDTO dto, Review oldReview)
        {

            oldReview.RateValue = dto.RateValue;
            oldReview.ReviewText = dto.ReviewText;
            oldReview.ReviewDate = DateTime.Now;
            return oldReview;
        }


    }
}
