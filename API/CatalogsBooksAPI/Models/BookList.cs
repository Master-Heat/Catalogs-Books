using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class BookList
    {
        public int ListID { get; set; }
        public int BookID { get; set; }

        //todo make sure to put virtual
        /*
        1- booklist 
        2- book
        and congiure them as foregin key
        as s [ForeignKey("ListID")] and as [BookID")]
        */
        [JsonIgnore]
        [ForeignKey("ListID")]
        virtual public UserList UserList { get; set; }
        [JsonIgnore]

        [ForeignKey("BookID")]
        virtual public Book Book { get; set; }

    }
}