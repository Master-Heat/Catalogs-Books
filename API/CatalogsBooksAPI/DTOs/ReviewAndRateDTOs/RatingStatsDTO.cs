namespace CatalogsBooksAPI.DTOs.ReviewAndRateDTOs
{
    class RatingStatsDTO
    {
        public int AverageRating { get; set; }
        public int TotalRate { get; set; }
        public List<string> RecentReviewers { get; set; }
    }
}