using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.CodeAnalysis.CSharp;

namespace CatalogsBooksAPI.Services.Factories
{
    public class BookDetailsFactory
    {
        BookDetailsRepo bookDetailsRepo;
        BooksRecsCardListFactory cardListFactory;
        RateAndReviewRepo rateAndReviewRepo;
        public BookDetailsFactory(BookDetailsRepo bookDetailsRepo,
        BooksRecsCardListFactory cardListFactory,
        RateAndReviewRepo rateAndReviewRepo)
        {
            this.bookDetailsRepo = bookDetailsRepo;
            this.cardListFactory = cardListFactory;
            this.rateAndReviewRepo = rateAndReviewRepo;
        }
        public async Task<BookDetailsDTO> GetBookDetails(int bookid)
        {
            // 1. Fetch the core book data
            var book = await bookDetailsRepo.GetBookById(bookid);
            if (book == null) return null;

            // 2. Fetch related details (Strings and IDs)
            (string mainCategory, string sbucategory) = await bookDetailsRepo.GetCategoryName(bookid);
            string authorName = await bookDetailsRepo.GetAuthorName(bookid);
            string seriesName = await bookDetailsRepo.GetSeriesName(bookid);

            // 3. get the count of the views
            int viewsCount = await bookDetailsRepo.GetBookViews(bookid);
            // 4. Use your GenerateRelatedList function for all the lists
            // We pass the method names from your repo as the 'Func' parameter
            List<BookCardDTO> sameAuthor = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.getBooksFromSameAutho);
            List<BookCardDTO> sameCategory = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.getBooksFromSameSubCategory);
            List<BookCardDTO> sameSeries = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.GetBooksInSameSeries);
            List<ReviewItemDTO> reviews = await rateAndReviewRepo.GetActiveReviewsWithUserInfoAsync(bookid);
            (double averageRate, int totalRateing) = await rateAndReviewRepo.GetBookRatingStatsAsync(bookid);
            int reviewCount;
            if (reviews == null)
            {
                reviewCount = 0;
            }
            else
            {
                reviewCount = reviews.Count;
            }
            // 5. Assemble the final DTO
            return new BookDetailsDTO
            {
                BookID = book.BookID,
                Title = book.Title,
                Description = book.Description,
                CoverImageLink = book.CoverImageLink,
                CoverAlt = book.CoverAlt,
                PublicationDate = book.PublicationDate,
                CanDownload = book.CanDownload,
                DownloadLink = book.DownloadLink,
                PagesCount = book.PagesCount,

                // Author Info
                AuthorID = book.AuthorID,
                AuthorName = authorName,
                FromSameAuthor = sameAuthor,

                // Category Info
                CategoryID = book.CategoryID,
                MainCategory = mainCategory,
                Sbucategory = sbucategory,
                FromSameSubcategory = sameCategory,

                // Series Info
                IsInSeire = !string.IsNullOrEmpty(seriesName),
                SeireName = seriesName,
                FromSameSeire = sameSeries ?? new List<BookCardDTO>(),

                // Views count 
                ViewsCount = viewsCount,
                Reviews = reviews,
                AverageRate = averageRate,
                TotalRatings = totalRateing,
                ActiveReviewCount = reviewCount

            };
        }



    }


}