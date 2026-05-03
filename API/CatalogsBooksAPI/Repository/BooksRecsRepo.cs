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
            return await _context.ViewedBooks
                        .Where(vb => vb.AccountID == accountID)
                        .Select(vb => vb.Book) // Get the book viewed
                        .SelectMany(b => _context.Books
                            .Where(allBooks => allBooks.CategoryID == b.CategoryID && allBooks.BookID != b.BookID))
                        .Distinct()
                        .ToListAsync();
        }
        public async Task<List<Book>> GetAuthorRecs(int accountID)
        {
            return await _context.ViewedBooks
                        .Where(vb => vb.AccountID == accountID)
                        .Select(vb => vb.Book) // Get the book viewed
                        .SelectMany(b => _context.Books
                            .Where(allBooks => allBooks.AuthorID == b.AuthorID && allBooks.BookID != b.BookID))
                        .Distinct()
                        .ToListAsync();
        }
        public async Task<List<Book>> GetAuthorAndCategoryRecs(int accountID)
        {
            return await _context.ViewedBooks
                        .Where(vb => vb.AccountID == accountID)
                        .Select(vb => vb.Book) // Get the book viewed
                        .SelectMany(b => _context.Books
                            .Where(allBooks =>
                            allBooks.AuthorID == b.AuthorID &&
                            allBooks.CategoryID == b.CategoryID &&
                            allBooks.BookID != b.BookID))
                        .Distinct()
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
