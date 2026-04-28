namespace CatalogsBooksAPI.Services.Factories
{
    using CatalogsBooksAPI.DTOs.AuthorDTOs;
    using CatalogsBooksAPI.Models;
    using Microsoft.EntityFrameworkCore;



    public class AuthorFactory
    {
        private readonly CatalogsBooksContext _context;

        public AuthorFactory(CatalogsBooksContext context)
        {
            _context = context;
        }

        // 1. Pure Search Logic
        public async Task<Author> FindAuthorByNameAsync(string authorName)
        {
            if (string.IsNullOrWhiteSpace(authorName)) return null;

            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorName.ToLower() == authorName.ToLower());
        }

        // 2. Creation Logic (Uses Search to prevent duplicates)
        public async Task<Author> CreateAuthorFromDTOAsync(AuthorCreateDTO authorDto)
        {
            // First, ensure the data is valid
            ValidateAuthorDTO(authorDto);

            // Second, check for existing author to prevent duplicates
            var existingAuthor = await FindAuthorByNameAsync(authorDto.AuthorName);
            if (existingAuthor != null)
            {
                return existingAuthor;
            }

            // Third, map to the model and add to context
            var newAuthor = new Author
            {
                AuthorName = authorDto.AuthorName,
                AccountID = null
            };

            _context.Authors.Add(newAuthor);

            return newAuthor;
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