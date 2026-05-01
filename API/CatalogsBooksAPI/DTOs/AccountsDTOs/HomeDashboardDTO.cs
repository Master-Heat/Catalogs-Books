using System.ComponentModel.DataAnnotations;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class HomeDashboardDTO
    {
        public string Token { get; set; }
        public string Name { get; set; }

        public List<BookCardDTO> CategoryRecs { get; set; }
        public List<BookCardDTO> AuthorRecs { get; set; }
        public List<BookCardDTO> AuthorAndCategoryRecs { get; set; }
        public List<BookCardDTO> PopularAllTime { get; set; }
        public List<BookCardDTO> PopularThisWeek { get; set; }
    }
}