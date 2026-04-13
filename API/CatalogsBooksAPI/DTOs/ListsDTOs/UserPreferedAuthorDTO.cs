using System.Runtime;
using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.ListsDTOs
{
    public class UserPreferedAuthorDTO
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public List<BookCardDTO> AuthorBooksCards { get; set; }
    }
}