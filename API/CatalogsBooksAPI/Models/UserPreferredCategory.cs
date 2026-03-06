using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CatalogsBooksAPI.Models
{
    public class UserPreferredCategory
    {
        public int AccountID { get; set; }

        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountID")]
        public Account Account { get; set; }



        [JsonIgnore]
        [ForeignKey("CategoryID")]
        virtual public Category Category { get; set; }

    }
}