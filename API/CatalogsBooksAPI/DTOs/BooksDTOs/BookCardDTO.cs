namespace CatalogsBooksAPI.DTOs.BooksDTOs
{
    public class BookCardDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageLink { get; set; }
        public string CoverAlt { get; set; }
    }
}