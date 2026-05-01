

using System.Threading.Tasks;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.DTOs.ListsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace CatalogsBooksAPI.Services.Factories
{

    public class ListsFactory
    {
        BookListRepo bookListRepo;
        public ListsFactory(BookListRepo bookListRepo)
        {
            this.bookListRepo = bookListRepo;
        }


        public async Task<List<GeneralUserListDTO>> GetAllUserListsWithBooks(int accountID)
        {
            // 1. Fetch everything in one trip to the database
            var userListsWithData = await bookListRepo.GetAllUserListsWithBooksIncluded(accountID);

            // 2. Map the entities directly to your DTOs
            return [.. userListsWithData.Select(list => new GeneralUserListDTO
    {
        ListID = list.ListID,
        ListName = list.ListName,
        // 3. Transform the included Book data into BookCardDTOs
        BookCards = [.. list.BookLists
            .Where(bl => bl.Book != null) // Safety check
            .Select(bl => new BookCardDTO
            {
                BookID = bl.Book.BookID,
                Title = bl.Book.Title,
                Description = bl.Book.Description,
                CoverImageLink = bl.Book.CoverImageLink,
                CoverAlt = bl.Book.CoverAlt
            })]
    })];



        }



        public async Task CreateEmptyUserList(int accountID, string listName)
        {
            // 1. Basic Validation
            if (string.IsNullOrWhiteSpace(listName))
            {
                throw new ArgumentException("List name cannot be empty.");
            }

            // 2. Check for duplicates using the updated repo method
            UserList exists = await bookListRepo.CheckIfListExist(accountID, listName.Trim());

            if (exists != null)
            {
                throw new ArgumentException($"A list with the name '{listName}' already exists.");
            }

            // 3. Create the new entity
            UserList newList = new UserList
            {
                AccountID = accountID,
                ListName = listName
            };

            // 4. Save to Database
            await bookListRepo.AddNewList(newList);
        }



        public async Task AddBookToList(int accountID, int listID, int bookID)
        {
            // 1. Security Check: Does this list actually belong to this user?
            List<UserList> userLists = await bookListRepo.GetAllUserLists(accountID);
            // Note: You might need a more specific 'GetListByIdAndAccount' repo method here

            bool userOwnsList = userLists.Any(l => l.ListID == listID);

            if (!userOwnsList)
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this list or the list does not exist.");
            }


            // 2. Duplicate Check: Is the book already there?
            bool alreadyExists = await bookListRepo.IsBookInList(listID, bookID);

            if (alreadyExists)
            {
                throw new ArgumentException("This book is already in your list.");
            }

            // 3. Execution
            await bookListRepo.AddBookToListAsync(listID, bookID);
        }



    }

}