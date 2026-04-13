using Microsoft.AspNetCore.Components.Web;

namespace CatalogsBooksAPI.DTOs.BooksDTOs
{
    public class GeneralChartDTO
    {
        public List<BookCardDTO> TopViewedAllTime { get; set; }
        public List<BookCardDTO> TopViewedThisYear { get; set; }


    }
}