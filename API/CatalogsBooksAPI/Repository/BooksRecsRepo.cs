using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
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


        public async Task<List<Book>> GetPopulatThisWeek()
        {
            // this function return top 30 viewed books this week 
            DateTime lastWeek = DateTime.UtcNow.AddDays(-7);

            return await _context.ViewedBooks.
            Where(v => v.ViewDate >= lastWeek).
            GroupBy(v => v.BookID).
            OrderByDescending(g => g.Count()).
            Take(30).
            Select(g => g.First().Book).
            ToListAsync();


        }
        public async Task<List<Book>> GetPopularAllTime()
        {
            return await _context.Books.
            OrderByDescending(b => b.ViewedBooks.Count()).
            Take(30).
            ToListAsync();

        }





    }
}