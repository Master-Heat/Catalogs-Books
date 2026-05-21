using System.Formats.Nrbf;
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

            string normalizedSearch = searchTerm.Trim().ToLower();
            // 1. Get the first 3 characters of each word
            List<string> prefixes = normalizedSearch
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Length <= 3 ? word : word.Substring(0, 3)) // Only take words long enough to have a prefix
                .Distinct()
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
                        .Include(b => b.Series)
                        .Where(b => prefixes.Any(p =>
                        b.Title.Contains(p) ||
                        b.Description.Contains(p) ||
                        (b.Series != null && b.Series.SeriesName.Contains(p))))
                .Take(200) // Safety limit
                .ToListAsync();



            // 3. Application Level: Fuzzy Sharp Ranking
            // We compare the FULL original searchTerm to the FULL candidate title
            List<Book> rankedResults = [.. candidates
                .Select(book =>
                {
                    string title = book.Title.ToLower().Trim() ?? "";
                    string series = book.Series?.SeriesName.ToLower().Trim() ?? "";
                    string desc = book.Description?.ToLower().Trim() ?? "";
                    // We use the null-conditional operator ?. to avoid crashes if  is null
                    
                    //handle typos 
                int titleTokenFuzz = Fuzz.TokenSetRatio(normalizedSearch, title);
                    // handle order 
                int titleSequenceFuzz = Fuzz.Ratio(normalizedSearch, title);

                double titleFuzz =((titleTokenFuzz * 0.98) + (titleSequenceFuzz * 0.2))* 1.0;
                double seriesFuzz =Fuzz.TokenSetRatio(normalizedSearch,series)*0.6;
                double descFuzz =Fuzz.TokenSetRatio(normalizedSearch,desc)*0.2;


            // Intent Multiplier Application (Handles Order Balance) 
            double titleMultiplier =1.0;
            if(title == normalizedSearch ) titleMultiplier = 3.0;
            else if ( title.StartsWith(normalizedSearch))titleMultiplier = 2.0;
            else if ( title.Contains(normalizedSearch))titleMultiplier = 1.5;

             double seriresMultiplier = 1.0;
            if (series == normalizedSearch) seriresMultiplier = 2.0;
            else if (series.Contains(normalizedSearch)) seriresMultiplier =1.2;

         double descMultiplier = 1.0;
            if(desc.Contains(normalizedSearch))descMultiplier =1.2;

                double finalScore = (titleFuzz * titleMultiplier)
                                    + (seriesFuzz * seriresMultiplier)
                                    + (descFuzz * descMultiplier);
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