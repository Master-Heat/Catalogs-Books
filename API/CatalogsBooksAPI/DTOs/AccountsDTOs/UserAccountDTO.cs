using System.ComponentModel.DataAnnotations;

namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class UserAccountDTO
    {
        public int AccountID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        public string AccessToken { get; set; }

    }
}