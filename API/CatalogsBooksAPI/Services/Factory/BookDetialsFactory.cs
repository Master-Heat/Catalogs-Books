using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.CodeAnalysis.CSharp;

namespace CatalogsBooksAPI.Services.Factories
{
    public class BookDetailsFactory
    {
        BookDetailsRepo bookDetailsRepo;
        BooksRecsCardListFactory cardListFactory;
        public BookDetailsFactory(BookDetailsRepo bookDetailsRepo,
        BooksRecsCardListFactory cardListFactory)
        {
            this.bookDetailsRepo = bookDetailsRepo;
            this.cardListFactory = cardListFactory;
        }
        public async Task<BookDetailsDTO> GetBookDetails(int bookid)
        {
            // 1. Fetch the core book data
            var book = await bookDetailsRepo.GetBookById(bookid);
            if (book == null) return null;

            // 2. Fetch related details (Strings and IDs)
            var categoryInfo = await bookDetailsRepo.GetCategoryName(bookid);
            var authorName = await bookDetailsRepo.GetAuthorName(bookid);
            var seriesName = await bookDetailsRepo.GetSeriesName(bookid);

            // 3. get the count of the views
            int viewsCount = await bookDetailsRepo.GetBookViews(bookid);
            // 4. Use your GenerateRelatedList function for all the lists
            // We pass the method names from your repo as the 'Func' parameter
            var sameAuthor = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.getBooksFromSameAutho);
            var sameCategory = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.getBooksFromSameSubCategory);
            var sameSeries = await cardListFactory.GenerateRelatedList(bookid, bookDetailsRepo.GetBooksInSameSeries);


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
                MainCategory = categoryInfo.MainCategory,
                Sbucategory = categoryInfo.Sbucategory,
                FromSameSubcategory = sameCategory,

                // Series Info
                IsInSeire = !string.IsNullOrEmpty(seriesName),
                SeireName = seriesName,
                FromSameSeire = sameSeries ?? new List<BookCardDTO>(),

                // Views count 
                ViewsCount = viewsCount,
            };
        }



    }


}