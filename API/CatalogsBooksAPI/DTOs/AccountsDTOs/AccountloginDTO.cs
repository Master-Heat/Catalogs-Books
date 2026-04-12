using System.ComponentModel.DataAnnotations;

namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class AccountloginDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}