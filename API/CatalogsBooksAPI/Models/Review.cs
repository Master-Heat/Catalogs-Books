using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class Review
    {
        [Key]

        public int ReviewID { get; set; }

        public int BookID { get; set; }
        public int AccountID { get; set; }
        public string ReviewText { get; set; }
        public double RateValue { get; set; }
        public DateTime ReviewDate { get; set; }

        [JsonIgnore]
        [ForeignKey("BookID")]
        public Book Book { get; set; }
        [JsonIgnore]
        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }

}