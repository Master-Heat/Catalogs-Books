namespace CatalogsBooksAPI.DTOs.AuthorDTOs
{
    public class AuthorCreateDTO
    {
        public string AuthorName { get; set; }
        public int? AccountId { get; set; } = null;
    }
}