using Microsoft.AspNetCore.Components.Web;

namespace CatalogsBooksAPI.DTOs.BooksDTOs
{
    public class CategoryChrtDTO
    {
        public string CategoryName { get; set; }
        public List<BookCardDTO> TopBooksInThisCategory { get; set; }

    }
}