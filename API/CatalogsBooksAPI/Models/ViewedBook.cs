using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class ViewedBook
    {
        public int AccountID { get; set; }
        public int BookID { get; set; }
        public DateTime ViewDate { get; set; }
        [JsonIgnore]
        [ForeignKey("BookID")]
        virtual public Book Book { get; set; }
        [JsonIgnore]
        [ForeignKey("AccountID")]
        virtual public Account Account { get; set; }

    }
}