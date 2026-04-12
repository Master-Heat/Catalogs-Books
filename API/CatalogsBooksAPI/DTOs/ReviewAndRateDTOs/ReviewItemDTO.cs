namespace CatalogsBooksAPI.DTOs.ReviewAndRateDTOs
{
    class ReviewItemDTO
    {
        public int ReviewID { get; set; }
        public int ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }

        // from account table 
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }

    }
}