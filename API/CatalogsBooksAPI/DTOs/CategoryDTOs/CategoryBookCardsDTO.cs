using CatalogsBooksAPI.DTOs.BooksDTOs;
using Microsoft.AspNetCore.Components.Web;

namespace CatalogsBooksAPI.DTOs.CategoryDTOs
{
    public class CategoryBookCardsDTO
    {
        public int CategoryID { get; set; }
        public string MainCategory { get; set; }
        public string Subcategory { get; set; }
        public List<BookCardDTO> BookCards { get; set; }


    }
}