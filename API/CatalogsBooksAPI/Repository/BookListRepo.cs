using System.Net.Http.Headers;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{
    public class BookListRepo
    {
        CatalogsBooksContext _context;
        public BookListRepo(CatalogsBooksContext context)
        {
            _context = context;
        }
        public async Task<List<UserList>> GetAllUserLists(int accountID)
        {
            return await _context.UserLists.Where(l => l.AccountID == accountID).ToListAsync();
        }



        public async Task<UserList> CheckIfListExist(int accountID, string listName)
        {
            return await _context.UserLists
                    .AsQueryable()
                    .FirstOrDefaultAsync(l => l.AccountID == accountID &&
                                        l.ListName == listName.Trim());
        }
        public async Task AddNewList(UserList list)
        {
            _context.UserLists.Add(list);
            _context.SaveChanges();
        }

        public async Task<List<Book>> GetBooksFromList(int listID)
        {
            return await _context.BookLists
                .Where(l => l.ListID == listID)
                .Include(l => l.Book).
                Select(l => l.Book)
                .ToListAsync();

        }

        public async Task<List<UserList>> GetAllUserListsWithBooksIncluded(int accountID)
        {
            return await _context.UserLists
                .Where(l => l.AccountID == accountID)
                .Include(l => l.BookLists) // Navigation property to the bridge table
                    .ThenInclude(bl => bl.Book) // Navigation property to the actual Book
                .ToListAsync();
        }



        public async Task<bool> IsBookInList(int listID, int bookID)
        {
            // Checks if this specific book is already linked to this specific list
            return await _context.BookLists
                .AnyAsync(bl => bl.ListID == listID && bl.BookID == bookID);
        }

        public async Task AddBookToListAsync(int listID, int bookID)
        {
            var bookListEntry = new BookList
            {
                ListID = listID,
                BookID = bookID
            };

            await _context.BookLists.AddAsync(bookListEntry);
            await _context.SaveChangesAsync();
        }
    }
}