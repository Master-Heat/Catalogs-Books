using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{
    public class AuthorRepo
    {
        CatalogsBooksContext _context;
        public AuthorRepo(CatalogsBooksContext context)
        {
            _context = context;
        }

        public async Task<Author> GetAuthorByID(int id)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorID == id);
        }
        public async Task<List<Author>> GetAllAuthors()
        {
            return await _context.Authors.ToListAsync();
        }
        public async Task<bool> AddAuthor(Author newAuthor)
        {
            _context.Authors.Add(newAuthor);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Author> GetAuthorByName(string name)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorName.ToLower() == name.ToLower());
        }
        public async Task<bool> DeleteAuthor(int id)
        {
            var author = await GetAuthorByID(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Book>> GetBooksByAuthorID(int authorId)
        {
            return await _context.Books
                .Where(b => b.AuthorID == authorId)
                .ToListAsync();
        }
    }
}