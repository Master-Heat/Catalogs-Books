using System.Runtime;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.ListsDTOs
{
    public class AddBookToListDTO
    {
        public int ListID { get; set; }
        public int BookID { get; set; }
    }
}