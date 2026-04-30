using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{

    public class RateAndReviewRepo
    {
        CatalogsBooksContext _context;
        public RateAndReviewRepo(CatalogsBooksContext context)
        {
            _context = context;
        }
        public async Task<Review> CheckIfUserReviewedThisBook(int bookid, int accountid)
        {
            return await _context.Reviews
                .AsQueryable()
                .FirstOrDefaultAsync(r => r.BookID == bookid && r.AccountID == accountid);
        }


        public async Task UpdateExistingReview(int ReviewID, string reviewText, double RateValue)
        {
            Review OldReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ReviewID == ReviewID);

            OldReview.RateValue = RateValue;
            OldReview.ReviewText = reviewText;
            OldReview.ReviewDate = DateTime.Now;

            _context.Reviews.Update(OldReview);
            _context.SaveChanges();

        }
        public async Task AddNewReview(Review rateAndReview)
        {


            _context.Reviews.Add(rateAndReview);
            _context.SaveChanges();
            return;
        }
    }
}
