namespace CatalogsBooksAPI.Models
{
    public class BookSeire
    {
        public int BookID { get; set; }
        public string SeireName { get; set; }
        //todo add virtual nullable list of book to add the relationship
    }
}