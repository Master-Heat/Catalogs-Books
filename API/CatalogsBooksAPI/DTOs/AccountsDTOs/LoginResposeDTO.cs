namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }

        public HomeDashboardDTO Dashboard { get; set; }
    }
}