using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CatalogsBooksAPI.Models
{
    public class UserList
    {
        [Key]
        public int ListID { get; set; }
        public int AccountID { get; set; }
        public string ListName { get; set; }


        // todo : for relation make 
        /* nullable virtual list of listitems 
            nullable virtual accont 
                 /? notice to put this above accounts [ForeignKey("AccountID")] to make sure it's configured the foregin key
         */
        [JsonIgnore]
        virtual public List<BookList> BookLists { get; set; }
        [JsonIgnore]
        [ForeignKey("AccountID")]
        virtual public Account Account { get; set; }


    }
}