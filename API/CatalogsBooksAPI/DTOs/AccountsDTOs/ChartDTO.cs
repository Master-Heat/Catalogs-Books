using System.ComponentModel.DataAnnotations;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.AccountsDTOs
{
    public class ChartDTO
    {

        public List<BookCardDTO> HighestRate { get; set; }
        public List<BookCardDTO> PopularAllTime { get; set; }
        public List<BookCardDTO> PopularThisWeek { get; set; }
    }
}