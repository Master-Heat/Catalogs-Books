namespace CatalogsBooksAPI.DTOs.AuthorDTOs
{
    public class AuthorDTO
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int? AccountId { get; set; } = null;
    }
}