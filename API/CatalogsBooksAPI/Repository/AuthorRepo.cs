using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogsBooksAPI.DTOs.AuthorDTOs;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogsBooksAPI.Repository
{
    public class AuthorRepo
    {
        private readonly CatalogsBooksContext _context;

        public AuthorRepo(CatalogsBooksContext context)
        {
            _context = context;
        }

        public async Task<AuthorDTO> FindAuthorByNameAsync(string authorName)
        {
            if (string.IsNullOrWhiteSpace(authorName)) return null;

            var author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AuthorName.ToLower() == authorName.ToLower());

            return author == null
                ? null
                : new AuthorDTO
                {
                    AuthorID = author.AuthorID,
                    AuthorName = author.AuthorName,
                    AccountId = author.AccountID
                };
        }

        public async Task<bool> CreateAuthorFromDTOAsync(AuthorCreateDTO authorDto)
        {
            ValidateAuthorDTO(authorDto);

            var existingAuthor = await FindAuthorByNameAsync(authorDto.AuthorName);
            if (existingAuthor != null)
            {
                return false;
            }

            var newAuthor = new Author
            {
                AuthorName = authorDto.AuthorName,
                AccountID = null
            };

            _context.Authors.Add(newAuthor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<AuthorDTO>> GetAllAuthorsAsync()
        {
            return await _context.Authors
                .AsNoTracking()
                .Select(a => new AuthorDTO
                {
                    AuthorID = a.AuthorID,
                    AuthorName = a.AuthorName,
                    AccountId = a.AccountID
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            return await DeleteAuthor(id);
        }

        public async Task<AuthorBooksDTO> GetAuthorWithBooksAsync(int authorId)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AuthorID == authorId);

            if (author == null) return null;

            var books = await _context.Books
                .AsNoTracking()
                .Where(b => b.AuthorID == authorId)
                .Select(b => new BookCardDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Description = b.Description,
                    CoverImageLink = b.CoverImageLink,
                    CoverAlt = b.CoverAlt
                })
                .ToListAsync();

            return new AuthorBooksDTO
            {
                AccountID = author.AuthorID,
                AuthorName = author.AuthorName,
                AuthorBookCards = books
            };
        }

        public async Task<List<AuthorBooksDTO>> GetAllAuthorsWithBooksAsync()
        {
            var authors = await _context.Authors
                .AsNoTracking()
                .ToListAsync();

            var authorBooksList = new List<AuthorBooksDTO>();

            foreach (var author in authors)
            {
                var books = await _context.Books
                    .AsNoTracking()
                    .Where(b => b.AuthorID == author.AuthorID)
                    .Select(b => new BookCardDTO
                    {
                        BookID = b.BookID,
                        Title = b.Title,
                        Description = b.Description,
                        CoverImageLink = b.CoverImageLink,
                        CoverAlt = b.CoverAlt
                    })
                    .ToListAsync();

                authorBooksList.Add(new AuthorBooksDTO
                {
                    AccountID = author.AccountID ?? 0,
                    AuthorName = author.AuthorName,
                    AuthorBookCards = books
                });
            }

            return authorBooksList;
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

        public async Task<bool> AddAuthor(AuthorCreateDTO newAuthor)
        {
            var author = new Author
            {
                AuthorName = newAuthor.AuthorName,
                AccountID = null
            };
            _context.Authors.Add(author);
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

        private void ValidateAuthorDTO(AuthorCreateDTO authorDto)
        {
            if (authorDto == null)
                throw new ArgumentNullException(nameof(authorDto), "Author data cannot be null.");

            if (string.IsNullOrWhiteSpace(authorDto.AuthorName))
                throw new ArgumentException("Author Name is required.");
        }
    }
}