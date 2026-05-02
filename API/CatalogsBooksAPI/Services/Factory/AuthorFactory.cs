namespace CatalogsBooksAPI.Services.Factories
{
    using CatalogsBooksAPI.DTOs.AuthorDTOs;
    using CatalogsBooksAPI.DTOs.BooksDTOs;
    using CatalogsBooksAPI.Models;
    using CatalogsBooksAPI.Repository;
    using Microsoft.EntityFrameworkCore;



    public class AuthorFactory
    {
        // private readonly CatalogsBooksContext _context;
        private readonly AuthorRepo _authorRepo;

        public AuthorFactory(AuthorRepo authorRepo)
        {
            _authorRepo = authorRepo;
        }

        // 1. Pure Search Logic
        public async Task<AuthorDTO> FindAuthorByNameAsync(string authorName)
        {
            if (string.IsNullOrWhiteSpace(authorName)) return null;

            var author = await _authorRepo.GetAuthorByName(authorName);

            return author == null ? null : new AuthorDTO
            {
                AuthorID = author.AuthorID,
                AuthorName = author.AuthorName,
                AccountId = author.AccountID
            };
        }

        // 2. Creation Logic (Uses Search to prevent duplicates)
        public async Task<bool> CreateAuthorFromDTOAsync(AuthorCreateDTO authorDto)
        {
            // First, ensure the data is valid
            ValidateAuthorDTO(authorDto);

            // Second, check for existing author to prevent duplicates
            var existingAuthor = await FindAuthorByNameAsync(authorDto.AuthorName);
            if (existingAuthor != null)
            {
                return false;
            }

            // Third, map to the model and add to context
            var newAuthor = new Author
            {
                AuthorName = authorDto.AuthorName,
                AccountID = null
            };

            return await _authorRepo.AddAuthor(newAuthor);
        }

        public async Task<List<AuthorDTO>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepo.GetAllAuthors();
            return [.. authors.Select(a => new AuthorDTO
            {
                AuthorID = a.AuthorID,
                AuthorName = a.AuthorName,
                AccountId = a.AccountID
            })];
        }
        public async Task<bool> DeleteAuthorAsync(int id)
        {
            return await _authorRepo.DeleteAuthor(id);
        }
        private void ValidateAuthorDTO(AuthorCreateDTO authorDto)
        {
            if (authorDto == null)
                throw new ArgumentNullException(nameof(authorDto), "Author data cannot be null.");

            if (string.IsNullOrWhiteSpace(authorDto.AuthorName))
                throw new ArgumentException("Author Name is required.");
        }

        public async Task<AuthorBooksDTO> GetAuthorWithBooksAsync(int authorId)
        {
            var author = await _authorRepo.GetAuthorByID(authorId);
            if (author == null) return null;

            var books = await _authorRepo.GetBooksByAuthorID(authorId);

            return new AuthorBooksDTO
            {
                AccountID = author.AuthorID,
                AuthorName = author.AuthorName,

                AuthorBookCards = books.Select(b => new BookCardDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Description = b.Description,
                    CoverImageLink = b.CoverImageLink,
                    CoverAlt = b.CoverAlt
                }).ToList()
            };
        }
        public async Task<List<AuthorBooksDTO>> GetAllAuthorsWithBooksAsync()
        {
            var authors = await _authorRepo.GetAllAuthors();
            var authorBooksList = new List<AuthorBooksDTO>();

            foreach (var author in authors)
            {
                var books = await _authorRepo.GetBooksByAuthorID(author.AuthorID);
                var authorBooks = new AuthorBooksDTO
                {
                    AccountID = author.AuthorID,
                    AuthorName = author.AuthorName,
                    AuthorBookCards = books.Select(b => new BookCardDTO
                    {
                        BookID = b.BookID,
                        Title = b.Title,
                        Description = b.Description,
                        CoverImageLink = b.CoverImageLink,
                        CoverAlt = b.CoverAlt
                    }).ToList()
                };
                authorBooksList.Add(authorBooks);
            }

            return authorBooksList;
        }
    }
}