namespace CatalogsBooksAPI.DTOs.BooksDTOs
{
    public class AlterBookDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public int AuthorID { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public int PagesCount { get; set; }
        public bool CanDownload { get; set; }
        public string DownloadLink { get; set; }
        public string CoverImageLink { get; set; }
        public string CoverAlt { get; set; }
        public DateOnly? PublicationDate { get; set; }
    }
}