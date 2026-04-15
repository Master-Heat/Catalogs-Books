// // API/CatalogsBooksAPI/Services/Factories/BookFactory.cs
// using CatalogsBooksAPI.DTOs.BooksDTOs;
// using CatalogsBooksAPI.Models;

// namespace CatalogsBooksAPI.Services.Factories
// {
//     /// <summary>
//     /// Factory Pattern: Encapsulates book creation logic
//     /// Replaces direct instantiation in controller
//     /// </summary>
//     public interface IBookFactory
//     {
//         Book CreateFromRequest(BookDetailsDTO request);
//     }


//     public class BookFactory : IBookFactory
//     {
//         private readonly CatalogsBooksContext _context;

//         public BookFactory(CatalogsBooksContext context)
//         {
//             _context = context;
//         }

//         public Book CreateFromRequest(BookDetailsDTO request)
//         {
//             ValidateRequest(request);

//             var book = new Book
//             {
//                 // AuthorID = request.AuthorId,
//                 Title = request.Title,
//                 PublicationDate = request.PublicationDate ?? default(DateOnly),
//                 CanDownload = request.CanDownload,
//                 DownloadLink = request.DownloadLink,
//                 Description = request.Description,
//                 CategoryID = request.CategoryID,
//                 CoverImageLink = request.CoverImageLink,
//                 CoverAlt = request.CoverAlt,
//                 PagesCount = request.PagesCount
//             };

//             return book;
//         }

//         private void ValidateRequest(BookDetailsDTO request)
//         {
//             if (string.IsNullOrWhiteSpace(request.Title))
//                 throw new ArgumentException("Title is required");

//             //todo make it rather that search author by id it search by name and if it didn't find any one create new author
//             if (request.AuthorID > 0 && !_context.Authors.Any(a => a.AuthorID == request.AuthorID))
//                 throw new ArgumentException("Invalid AuthorID");
//             //todo : create if author is not exist create new author with the same name

//             if (request.CategoryID > 0 && !_context.Categories.Any(c => c.CategoryID == request.CategoryID))
//                 throw new ArgumentException("Invalid CategoryID");
//             //todo : create if category is not exist create new category with the same name
//         }
//     }
// }