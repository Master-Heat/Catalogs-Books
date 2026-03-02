namespace CatalogsBooksAPI.Models
{
    public class UserPreferedCategory
    {
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

    }
}