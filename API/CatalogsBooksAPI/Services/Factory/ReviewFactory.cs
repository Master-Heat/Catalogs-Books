namespace CatalogsBooksAPI.Services.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
    using CatalogsBooksAPI.Models;
    using CatalogsBooksAPI.Repository;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Update.Internal;


    public class ReviewFactory
    {
        private readonly CatalogsBooksContext _context;
        private readonly RateAndReviewRepo rateAndReviewRepo;

        public ReviewFactory(CatalogsBooksContext context,
        RateAndReviewRepo rateAndReviewRepo)
        {
            _context = context;
            this.rateAndReviewRepo = rateAndReviewRepo;
        }

        public async Task CreateOrUpdateReviewAsync(int accountid, int bookid, string reviewText, double RateValue)
        {
            // 1. Validation Logic
            await ValidateReviewDTO(bookid, accountid, RateValue);

            // 2. Search Logic: Check if this user already reviewed this book
            Review existingReview = await rateAndReviewRepo.CheckIfUserReviewedThisBook(bookid, accountid);

            if (existingReview != null)
            {

                await rateAndReviewRepo.UpdateExistingReview(existingReview.ReviewID, reviewText, RateValue);
                return;
            }

            // 3. Creation Logic: If not found, create a new recor
            Review newRateAndReview = new Review
            {
                AccountID = accountid,
                BookID = bookid,
                ReviewDate = DateTime.Now,
                ReviewText = reviewText,
                RateValue = RateValue
            };

            await rateAndReviewRepo.AddNewReview(newRateAndReview);


        }

        private async Task ValidateReviewDTO(int bookid, int accountid, double RateValue)
        {


            if (bookid <= 0 || accountid <= 0)
                throw new ArgumentException("Valid BookID and AccountID are required.");

            // Ensure the rate is within a realistic scale (e.g., 1 to 5)
            if (RateValue < 0 || RateValue > 5)
                throw new ArgumentException("Rating must be between 0 and 5.");


        }
        public Review updateExistingReview(Review OldReview, string reviewText, double RateValue)
        {

            OldReview.RateValue = RateValue;
            OldReview.ReviewText = reviewText;
            OldReview.ReviewDate = DateTime.Now;
            return OldReview;
        }
        public async Task<List<RateAndReviewDTO>> GetAllBookReviews(int bookid)
        {
            List<Review> reviews = await rateAndReviewRepo.GetBookReviews(bookid);
            if (reviews == null) return null;
            return [.. reviews.Select(r => new RateAndReviewDTO{
                BookID = r.BookID,
                AccountID = r.AccountID,
                ReviewText = r.ReviewText,
                RateValue = r.RateValue,
                ReviewDate = r.ReviewDate
            })


            ];
        }
        public async Task<RateAndReviewDTO> GetReviewDTOByID(int reivewid)
        {
            Review review = await rateAndReviewRepo.GetReviewByID(reivewid);
            if (review == null) return null;
            return new RateAndReviewDTO
            {
                BookID = review.BookID,
                AccountID = review.AccountID,
                ReviewText = review.ReviewText,
                RateValue = review.RateValue,
                ReviewDate = review.ReviewDate
            };

        }

    }
}
