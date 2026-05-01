using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class Series
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BookID { get; set; }
        public string SeriesName { get; set; }

        [JsonIgnore]
        [ForeignKey("BookID")]
        virtual public Book Books { get; set; }
    }
}