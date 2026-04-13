using System.Runtime;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.ListsDTOs
{
    public class UserPreferedCategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<BookCardDTO> AuthorBooksCards { get; set; }
    }
}