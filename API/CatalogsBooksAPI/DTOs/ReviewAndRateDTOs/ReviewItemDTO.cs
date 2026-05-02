namespace CatalogsBooksAPI.DTOs.ReviewAndRateDTOs
{
    public class ReviewItemDTO
    {
        public int ReviewID { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }

        // from account table 
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

    }
}