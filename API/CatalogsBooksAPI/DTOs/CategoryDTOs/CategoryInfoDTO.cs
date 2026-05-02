using Microsoft.AspNetCore.Components.Web;

namespace CatalogsBooksAPI.DTOs.CategoryDTOs
{
    public class CategoryInfoDTO
    {
        public int CategoryID { get; set; }
        public string MainCategory { get; set; }
        public string Subcategory { get; set; }


    }
}