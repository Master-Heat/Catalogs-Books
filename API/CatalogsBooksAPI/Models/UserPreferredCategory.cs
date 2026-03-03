using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class UserPreferredCategory
    {
        public int AccountID { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountID")]
        public Account Account { get; set; }

        [JsonIgnore]
        public Book Book { get; set; }

    }
}