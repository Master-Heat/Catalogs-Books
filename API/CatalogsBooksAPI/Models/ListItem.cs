namespace CatalogsBooksAPI.Models
{
    public class ListItem
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
    }
}