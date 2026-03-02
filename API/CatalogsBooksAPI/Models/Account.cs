using System.ComponentModel.DataAnnotations;

namespace CatalogsBooksAPI.Models
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PermissionLevel { get; set; }

        // public string? UserPrefrences { get; set; }

        //todo:add public nullable  virtual lists of
        /*          1- books
                    2- Reviews 
                    3- ViewedBooks
                    4- UserPreferedAuthors
                    5- UserPreferedCategories
                 */

    }
}