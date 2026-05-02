using CatalogsBooksAPI.DTOs.BooksDTOs;
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
        public async Task<List<ReviewItemDTO>> GetActiveReviewsWithUserInfoAsync(int bookid)
        {
            return await _context.Reviews
        .Where(r => r.BookID == bookid && !string.IsNullOrWhiteSpace(r.ReviewText))
        .Select(r => new ReviewItemDTO
        {
            ReviewDate = r.ReviewDate,
            ReviewID = r.ReviewID,
            Email = r.Account.Email,
            UserName = r.Account.UserName,
            Role = r.Account.Role,
            ReviewText = r.ReviewText
        })
        .ToListAsync();
        }

        public async Task<(double AverageRate, int TotalRatings)> GetBookRatingStatsAsync(int bookId)
        {
            var stats = await _context.Reviews
                .Where(r => r.BookID == bookId)
                .GroupBy(r => r.BookID)
                .Select(g => (new
                {


                    AverageRate = g.Average(r => r.RateValue),
                    TotalRatings = g.Count()
                }
                )
                )
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                return (0.0, 0);
            }
            return (stats.AverageRate, stats.TotalRatings);
        }

        public async Task<List<UserReviewDTO>> GetUserReviews(int accountid)
        {
            return await _context.Reviews

         .Where(r => r.AccountID == accountid && !string.IsNullOrWhiteSpace(r.ReviewText))
         .Select(r => new UserReviewDTO
         {
             ReviewID = r.ReviewID,
             RateValue = r.RateValue,
             ReviewText = r.ReviewText,
             ReviewDate = r.ReviewDate,

             // Mapping the nested BookCardDTO directly from the navigation property
             ReviewedBookCard = new BookCardDTO
             {
                 BookID = r.Book.BookID,
                 Title = r.Book.Title,
                 Description = r.Book.Description,
                 CoverImageLink = r.Book.CoverImageLink,
                 CoverAlt = r.Book.CoverAlt
             }
         })
         .ToListAsync();
        }


        public async Task<List<Review>> GetBookReviews(int bookid)
        {
            return await _context.Reviews
            .Where(r => r.BookID == bookid && !string.IsNullOrWhiteSpace(r.ReviewText))
            .ToListAsync();
        }
    }
}
