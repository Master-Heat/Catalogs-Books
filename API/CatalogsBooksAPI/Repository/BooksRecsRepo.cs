using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{
    public class BooksRecsRepo
    {
        CatalogsBooksContext _context;

        public BooksRecsRepo(CatalogsBooksContext context)
        {
            _context = context;


        }
        public async Task<List<Book>> GetCategoryRecs(int accountID)
        {


            var viewedBookIds = _context.ViewedBooks
          .Where(vb => vb.AccountID == accountID)
          .Select(vb => vb.BookID);

            var categoryIds = _context.Books
                .Where(b => viewedBookIds.Contains(b.BookID))
                .Select(b => b.CategoryID)
                .Distinct();

            return await _context.Books
                .Where(b => categoryIds.Contains(b.CategoryID))
                .Where(b => !viewedBookIds.Contains(b.BookID)) // optional: exclude already viewed books
                .ToListAsync();
        }
        public async Task<List<Book>> GetAuthorRecs(int accountID)
        {
            var viewedBookIds = _context.ViewedBooks
        .Where(vb => vb.AccountID == accountID)
        .Select(vb => vb.BookID);

            var authorIds = _context.Books
                .Where(b => viewedBookIds.Contains(b.BookID))
                .Select(b => b.AuthorID)
                .Distinct();

            return await _context.Books
                .Where(b => authorIds.Contains(b.AuthorID))
                .Where(b => !viewedBookIds.Contains(b.BookID))
                .Include(b => b.Author)     // only if Book has Author navigation
                .Include(b => b.Category)   // only if Book has Category navigation
                .Take(20)
                .ToListAsync();
        }
        public async Task<List<Book>> GetAuthorAndCategoryRecs(int accountID)
        {
            var viewedBookIds = _context.ViewedBooks
         .Where(vb => vb.AccountID == accountID)
         .Select(vb => vb.BookID);

            var authorCategoryPairs = _context.Books
                .Where(b => viewedBookIds.Contains(b.BookID))
                .Select(b => new { b.AuthorID, b.CategoryID })
                .Distinct();

            return await _context.Books
                .AsNoTracking()
                .Where(b => authorCategoryPairs
                    .Any(p => p.AuthorID == b.AuthorID && p.CategoryID == b.CategoryID))
                .Where(b => !viewedBookIds.Contains(b.BookID))
                .Include(b => b.Author)     // remove if no Author navigation in Book
                .Include(b => b.Category)   // remove if no Category navigation in Book
                .OrderByDescending(b => b.BookID) // optional, but better with Take
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Book>> GetBookByHighestRate()
        {
            return await _context.Reviews
        .GroupBy(r => r.BookID)
        .Select(group => new
        {
            BookID = group.Key,
            AverageRating = group.Average(r => r.RateValue)
        })
        .OrderByDescending(x => x.AverageRating)
        .Join(_context.Books
                .Include(b => b.Author)   // Must load to avoid line 46 crash
                .Include(b => b.Category), // Must load to avoid line 46 crash
              reviewResult => reviewResult.BookID,
              book => book.BookID,
              (reviewResult, book) => book) // Directly returns the Book object
        .ToListAsync();
        }
    }


}
