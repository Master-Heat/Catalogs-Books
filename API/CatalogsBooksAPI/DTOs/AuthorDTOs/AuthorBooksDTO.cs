using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.AuthorDTOs
{
    public class AuthorBooks
    {


        public int? AccountID { get; set; }

        public string AuthorName { get; set; }

        public List<BookCardDTO> AuthorBookCards { get; set; }


    }
}