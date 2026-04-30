namespace CatalogsBooksAPI.DTOs.ReviewAndRateDTOs
{
    public class RateAndReviewDTO
    {


        public int BookID { get; set; }
        public int AccountID { get; set; }
        public string ReviewText { get; set; }
        public double RateValue { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}