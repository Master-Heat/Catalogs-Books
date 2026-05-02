using CatalogsBooksAPI.DTOs.ReviewAndRateDTOs;

namespace CatalogsBooksAPI.DTOs.BooksDTOs
{
    public class BookDetailsDTO
    {

        public int BookID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageLink { get; set; }
        public string CoverAlt { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public bool CanDownload { get; set; }
        public string DownloadLink { get; set; }
        public int PagesCount { get; set; }

        // from authortable
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public List<BookCardDTO> FromSameAuthor { get; set; }
        //from category
        public int CategoryID { get; set; }
        public string MainCategory { get; set; }
        public string Sbucategory { get; set; }
        public List<BookCardDTO> FromSameSubcategory { get; set; }


        // from seires
        public bool IsInSeire { get; set; }
        public string SeireName { get; set; }
        public List<BookCardDTO> FromSameSeire { get; set; }

        // from query with viewed books 
        public int ViewsCount { get; set; }
        // from query with review 
        public List<ReviewItemDTO> Reviews { get; set; }
        public double AverageRate { get; set; }
        public int TotalRatings { get; set; }
        public int ActiveReviewCount { get; set; }

    }
}