using System.Runtime;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.ListsDTOs
{
    public class SeireDTO
    {
        public string SeireName { get; set; }
        public List<BookCardDTO> AuthorBooksCards { get; set; }
    }
}