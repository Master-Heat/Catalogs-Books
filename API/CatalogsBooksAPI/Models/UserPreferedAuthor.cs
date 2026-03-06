using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CatalogsBooksAPI.Models
{
    public class UserPreferedAuthor
    {
        public int AccountID { get; set; }
        public int AuthorID { get; set; } // refrence the user that prefere that author

        [JsonIgnore]
        [ForeignKey("AuthorID")]
        virtual public Author Author { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountID")]
        virtual public Account Account { get; set; }
    }
}