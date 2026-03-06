using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CatalogsBooksAPI.Models
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }

        public int? AccountID { get; set; }
        [Required]
        public string AuthorName { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountID")]
        public Account Account { get; set; }

        [JsonIgnore]
        public List<Book> Books { get; set; }

        [JsonIgnore]
        virtual public List<UserPreferedAuthor> UserPreferedAuthor { get; set; }

    }
}