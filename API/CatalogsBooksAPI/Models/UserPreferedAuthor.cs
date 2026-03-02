namespace CatalogsBooksAPI.Models
{
    public class UserPreferedAuthor
    {
        public string AuthorName { get; set; }
        public string AccountID { get; set; } // refrence the user that prefere that author
        // todo make the relationships make 
        /*
        add nullable Account and link it to foreign key AccountID [ForeignKey("AccountID")]
        add nullable Book and link it to foreign key AuthorName   [ForeignKey("AuthorName")]

        */
    }
}