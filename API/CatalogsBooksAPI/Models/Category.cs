using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CatalogsBooksAPI.Models.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string MainCategory { get; set; }
        public string Sbucategory { get; set; }

        [JsonIgnore]
        virtual public List<Book> Books { get; set; }
        [JsonIgnore]
        virtual public List<UserPreferredCategory> UserPreferredCategories { get; set; }
    }
}