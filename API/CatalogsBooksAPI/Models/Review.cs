using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        // todo for relationships create the following  vitual things
        /*
        1- Review  
        2- Account
            and configure the foreign keys  [ForeignKey("BookID")], [ForeignKey("AccountID")]
        */
    }

}