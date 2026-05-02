using System.Security.AccessControl;
using CatalogsBooksAPI.DTOs.BooksDTOs;
namespace CatalogsBooksAPI.DTOs.ReviewAndRateDTOs
{
    public class UserReviewDTO
    {
        public int ReviewID { get; set; }
        public double RateValue { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }

        public BookCardDTO ReviewedBookCard { get; set; }
    }
}