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
        public string AuthorName { get; set; }
        //from category
        public string MainCategory { get; set; }
        public string Sbucategory { get; set; }
        // from seires
        public bool IsInSeire { get; set; }
        public string SeireName { get; set; }

        // from query with viewed books 
        public int ViewsCount { get; set; }

    }
}