using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Repository
{
    public class BookDetailsRepo
    {
        CatalogsBooksContext _context;
        public BookDetailsRepo(CatalogsBooksContext context)
        {
            _context = context;
        }

        public async Task<Book> GetBookById(int bookid)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.BookID == bookid);
        }
        public async Task<(string MainCategory, string Sbucategory)> GetCategoryName(int bookId)
        {
            Category category = await _context.Books.
                Where(b => b.BookID == bookId)
                .Select(b => b.Category).
                FirstOrDefaultAsync();

            return (category.MainCategory, category.Sbucategory);
        }

        public async Task<string> GetAuthorName(int bookId)
        {
            Author author = await _context.Books
            .Where(b => b.BookID == bookId)
            .Select(b => b.Author)
            .FirstOrDefaultAsync();

            return author.AuthorName;
        }


        public async Task<List<Book>> getBooksFromSameSubCategory(int bookId)
        {
            var book = await GetBookById(bookId);
            return await _context.Books
                .Where(b => b.CategoryID == book.CategoryID && b.BookID != bookId)
                .ToListAsync();
        }
        public async Task<List<Book>> getBooksFromSameAutho(int bookId)
        {
            var book = await GetBookById(bookId);
            return await _context.Books
                .Where(b => b.AuthorID == book.AuthorID && b.BookID != bookId)
                .ToListAsync();
        }

        public async Task<string> GetSeireName(int bookId)
        {
            Seire seire = await _context.Books
            .Where(b => b.BookID == bookId)
            .Select(b => b.Seire)
            .FirstOrDefaultAsync();
            if (seire == null) return null;
            return seire.SeireName;

        }
        public async Task<List<Book>> GetBooksInSameSeire(int bookId)
        {
            string seireName = await GetSeireName(bookId); // Note the lowercase variable
            if (seireName == null) return null;

            return await _context.Seires
                    .Where(s => s.SeireName == seireName && s.BookID != bookId) // Use variable here
                    .Include(s => s.Books)
                    .Select(s => s.Books)
                    .ToListAsync();

        }
        public async Task<int> GetBookViews(int bookid)
        {
            Book book = await GetBookById(bookid);

            if (book == null || book.ViewedBooks == null)
            {
                return 0;
            }
            return await _context.ViewedBooks
                    .CountAsync(v => v.BookID == bookid);
        }
    }
}