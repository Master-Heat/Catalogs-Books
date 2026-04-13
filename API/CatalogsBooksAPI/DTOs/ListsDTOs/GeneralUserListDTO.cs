using CatalogsBooksAPI.DTOs.BooksDTOs;

namespace CatalogsBooksAPI.DTOs.ListsDTOs
{
    public class GeneralUserListDTO
    {
        public int ListID { get; set; }
        public string ListName { get; set; }

        public List<BookCardDTO> BookCards { get; set; }
    }
}