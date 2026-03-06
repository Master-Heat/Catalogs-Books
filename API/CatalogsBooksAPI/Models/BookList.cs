using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class BookList
    {
        public int ListID { get; set; }
        public int BookID { get; set; }

        [JsonIgnore]
        [ForeignKey("ListID")]
        virtual public UserList UserList { get; set; }
        [JsonIgnore]

        [ForeignKey("BookID")]
        virtual public Book Book { get; set; }

    }
}