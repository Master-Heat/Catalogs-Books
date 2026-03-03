using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogsBooksAPI.Models
{
    public class ViewedBook
    {
        public int AccountID { get; set; }
        public int BookID { get; set; }
        // todo to make the relation add these virtual attributes
        /*
            nullable type of Book
            nullable type of Account
                    make user to configure the foregin keys as the following  
                        [Foreingkey("BookID")] and [ForeignKey(AccountID)]
        */
        [JsonIgnore]
        [ForeignKey("BookID")]
        virtual public Book Book { get; set; }
        [JsonIgnore]
        [ForeignKey("AccountID")]
        virtual public Account Account { get; set; }
    }
}