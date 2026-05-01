using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // Default value

        public string AccountState { get; set; } = "Active"; // Default value

        [JsonIgnore]
        virtual public Author Author { get; set; }
        [JsonIgnore]
        virtual public List<Review> Reviews { get; set; }
        [JsonIgnore]
        virtual public List<ViewedBook> ViewedBooks { get; set; }
        [JsonIgnore]
        virtual public List<UserPreferedAuthor> UserPreferedAuthors { get; set; }
        [JsonIgnore]
        virtual public List<UserPreferredCategory> UserPreferedCategories { get; set; }
        [JsonIgnore]
        virtual public List<UserList> UserLists { get; set; }

    }
}