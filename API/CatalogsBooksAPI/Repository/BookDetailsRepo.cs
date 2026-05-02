using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public async Task<string> GetSeriesName(int bookId)
        {
            Series series = await _context.Books
            .Where(b => b.BookID == bookId)
            .Select(b => b.Series)
            .FirstOrDefaultAsync();
            if (series == null) return null;
            return series.SeriesName;

        }
        public async Task<List<Book>> GetBooksInSameSeries(int bookId)
        {
            string seriesName = await GetSeriesName(bookId); // Note the lowercase variable
            if (seriesName == null) return null;

            return await _context.Series
                    .Where(s => s.SeriesName == seriesName && s.BookID != bookId) // Use variable here
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

        public async Task<Book> AddBook(Book book)
        {
            Book oldBook = await GetBookById(book.BookID);
            if (oldBook != null) return null;

            await _context.Books.AddAsync(book);
            _context.SaveChanges();
            return book;

        }

        public async Task<Book> GetBookbyTitle(string bookTitle)
        {
            Book book = await _context.Books
            .FirstOrDefaultAsync(c =>
            c.Title.ToLower() == bookTitle.ToLower());

            return book;
        }


        public async Task<List<Book>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }
        public async Task<bool> AlterExisitngBook(Book book)
        {
            Book exisitingBook = await GetBookById(book.BookID);
            if (exisitingBook == null) return false;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteBook(int bookId)
        {
            Book book = await GetBookById(bookId);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}