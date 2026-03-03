using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class UserPreferedAuthor
    {
        public string AuthorName { get; set; }
        public int AccountID { get; set; } // refrence the user that prefere that author

        [JsonIgnore]
        [ForeignKey("AccountID")]
        virtual public Account Account { get; set; }

        [JsonIgnore]
        virtual public Book Book { get; set; }
    }
}