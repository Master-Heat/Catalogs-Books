using System.ComponentModel.DataAnnotations;

namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class AccountRegisterDTO
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}