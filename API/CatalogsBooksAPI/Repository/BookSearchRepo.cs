using CatalogsBooksAPI.Models;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace CatalogsBooksAPI.Repository
{
    public class BookSearchRepo
    {
        CatalogsBooksContext _context;
        public BookSearchRepo(CatalogsBooksContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetSmartSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<Book>();

            // 1. Get the first 3 characters of each word
            List<string> prefixes = searchTerm.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.Length >= 3) // Only take words long enough to have a prefix
                .Select(word => word.Substring(0, 2))
                .ToList();

            if (!prefixes.Any())
            {
                // Fallback: If search term is very short (e.g. "It"), just do a standard contains
                return await _context.Books
                    .Where(b => b.Title.Contains(searchTerm))
                    .ToListAsync();
            }

            // 2. Database Level: Broad Filter
            // Find any book title, description , serirename  containing any of the 3-char prefixes
            List<Book> candidates = await _context.Books
                        .Include(b => b.Seire)
                        .Where(b => prefixes.Any(p =>
                        b.Title.Contains(p) ||
                        b.Description.Contains(p) ||
                        (b.Seire != null && b.Seire.SeireName.Contains(p))))
                .Take(200) // Safety limit
                .ToListAsync();



            // 3. Application Level: Fuzzy Sharp Ranking
            // We compare the FULL original searchTerm to the FULL candidate title
            List<Book> rankedResults = [.. candidates
                .Select(book =>
                {
                    // We use the null-conditional operator ?. to avoid crashes if Seire is null
                int titleScore = Fuzz.TokenSetRatio(searchTerm, book.Title);
                int seriesScore = Fuzz.TokenSetRatio(searchTerm, book.Seire?.SeireName ?? "");
                int descScore = Fuzz.TokenSetRatio(searchTerm, book.Description ?? "");
                double finalScore = (titleScore * 1.0) + (seriesScore * 0.7) + (descScore * 0.4);
                    return new
                    {
                        Book = book,
                        // WeightedRatio is excellent for comparing the overall "feel" of two strings
                        Score = finalScore
                    };
                })
                .Where(x => x.Score > 40)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Book)];

            return rankedResults;
        }
    }
}