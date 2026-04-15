


namespace CatalogsBooksAPI.Services.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using CatalogsBooksAPI.DTOs.BooksDTOs;
    using CatalogsBooksAPI.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IBookFactory
    {
        Task<Book> CreateFromDTO(CreateBookDTO dto);
    }
    public class BookFactory : IBookFactory
    {
        private readonly CatalogsBooksContext _context;
        // public CategoryFactory(CatalogsBooksContext context)
        // {
        //     _context = context;
        // }
        public async Task<Book> CreateFromDTO(CreateBookDTO dto)
        {
            // 1. Validate the incoming data
            ValidateBookDTO(dto);

            var existingBook = await FindBookAsync(dto.Title);
            if (existingBook != null)
            {
                return existingBook;
            }

            // 2. Map DTO to Model

            return new Book
            {
                Title = dto.Title,
                AuthorID = dto.AuthorID,
                CategoryID = dto.CategoryID,
                Description = dto.Description,
                PagesCount = dto.PagesCount,
                CanDownload = dto.CanDownload,
                DownloadLink = dto.DownloadLink,
                CoverImageLink = dto.CoverImageLink,
                CoverAlt = dto.CoverAlt,

                // Handle the nullable DateOnly - default to today if null
                PublicationDate = dto.PublicationDate ?? DateOnly.FromDateTime(DateTime.Now)
            };
        }

        private void ValidateBookDTO(CreateBookDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Book data cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Book Title is required.");

            if (dto.AuthorID <= 0)
                throw new ArgumentException("A valid Author must be selected.");

            if (dto.CategoryID <= 0)
                throw new ArgumentException("A valid Category must be selected.");

            if (dto.PagesCount <= 0)
                throw new ArgumentException("Pages count cannot be negative or zero");

            // Logic validation: If it's downloadable, it must have a link
            if (dto.CanDownload && string.IsNullOrWhiteSpace(dto.DownloadLink))
                throw new ArgumentException("Download link is required if the book is set to 'Can Download'.");
        }
        public async Task<Book> FindBookAsync(string bookTitle)
        {
            return await _context.Books
           .FirstOrDefaultAsync(c =>
           c.Title.ToLower() == bookTitle.ToLower());
        }
    }
}