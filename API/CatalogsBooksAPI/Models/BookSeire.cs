using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class BookSeire
    {
        [Key]
        public int SeireID { get; set; }
        public string SeireName { get; set; }

        [JsonIgnore]
        virtual public List<Book> Books { get; set; }
    }
}