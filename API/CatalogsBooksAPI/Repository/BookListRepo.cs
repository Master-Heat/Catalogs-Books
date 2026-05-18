using System.Net.Http.Headers;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.DTOs.ListsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
            // return await _context.UserLists.Where(l => l.AccountID == accountID).ToListAsync();






            return await _context.UserLists.Where(l => l.AccountID == accountID).ToListAsync();
        }




        public async Task AddNewList(string listName, int accountID)
        {
            // _context.UserLists.Add(list);
            // _context.SaveChanges();

            if (string.IsNullOrWhiteSpace(listName))
            {

                throw new ArgumentException("List Name is Required");
            }

            UserList existlist = await _context.UserLists
                    .FirstOrDefaultAsync(l => l.AccountID == accountID
                                        && l.ListName.Trim() == listName.Trim());
            if (existlist != null)
            {
                throw new ArgumentException("List is Already exist \nCan't Have Two Lists With The Same Name");
            }
            UserList userList = new UserList
            {
                AccountID = accountID,
                ListName = listName.Trim()
            };
            _context.UserLists.Add(userList);
            _context.SaveChanges();



        }

        public async Task<List<GeneralUserListDTO>> GetAllUserListsWithBooksIncluded(int accountID)
        {
            // return await _context.UserLists
            //     .Where(l => l.AccountID == accountID)
            //     .Include(l => l.BookLists) // Navigation property to the bridge table
            //         .ThenInclude(bl => bl.Book) // Navigation property to the actual Book
            //     .ToListAsync();








            return await _context.UserLists
                .Where(l => l.AccountID == accountID)
                .Select(list => new GeneralUserListDTO
                {
                    ListID = list.ListID,
                    ListName = list.ListName,
                    BookCards = list.BookLists.Where(bl => bl.Book != null)
                    .Select(bl => new BookCardDTO
                    {
                        BookID = bl.BookID,
                        Title = bl.Book.Title,
                        Description = bl.Book.Description,
                        CoverImageLink = bl.Book.CoverImageLink,
                        CoverAlt = bl.Book.CoverAlt
                    }).ToList()
                }).ToListAsync();
            // .Include(l => l.BookLists)
            // .ThenInclude(bl => bl.Book)
            // .ToListAsync();
        }



        public async Task AddBookToListAsync(int? listID, int? bookID, int? accoutID)
        {
            if (listID == null || bookID == null || accoutID == null)
            {
                throw new ArgumentException("book id, account id and list id are required");
            }
            var validation = await _context.UserLists
                            .Where(l => l.ListID == listID && l.AccountID == accoutID)
                            .Select(l => new
                            {

                                bookExist = _context.Books.Any(b => b.BookID == bookID),
                                AlreadyInList = _context.BookLists.Any(l => l.ListID == listID && l.BookID == bookID)
                            }).FirstOrDefaultAsync();

            if (validation == null)
                throw new UnauthorizedAccessException("You don't have access to this list");
            if (!validation.bookExist)
                throw new ArgumentException("This book doesn't exist");
            if (validation.AlreadyInList) return;
            // bool bookExists = await _context.Books.AnyAsync(b => b.BookID == bookID);
            // if (!bookExists)
            // {
            //     throw new ArgumentException("This book doesn't exist");
            // }

            // bool ifUserOwnTheList = await _context.UserLists.AnyAsync(l => l.AccountID == accoutID && l.ListID == listID);
            // if (!ifUserOwnTheList)
            // {

            //     throw new UnauthorizedAccessException("You don't have access to this list");
            // }
            var bookListEntry = new BookList
            {
                ListID = (int)listID,
                BookID = (int)bookID
            };
            bool bookExisitInList = await _context.BookLists.AnyAsync(l => l.ListID == listID && l.BookID == bookID);
            if (bookExisitInList)
            {
                return;
            }

            await _context.BookLists.AddAsync(bookListEntry);
            await _context.SaveChangesAsync();
        }


    }
}