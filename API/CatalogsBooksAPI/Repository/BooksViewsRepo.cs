using System.Data;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
namespace CatalogsBooksAPI.Repository
{
    public class BookviewsRepo
    {
        CatalogsBooksContext _context;
        AccountRepo accountRepoe;
        BookDetailsRepo bookDetailsRepo;
        public BookviewsRepo(CatalogsBooksContext context,

           AccountRepo accountRepo,
        BookDetailsRepo bookDetailsRepo)
        {
            _context = context;
            this.accountRepoe = accountRepo;
            this.bookDetailsRepo = bookDetailsRepo;

        }

        public async Task<List<Book>> GetPopulatThisWeek()
        {
            // this function return top 30 viewed books this week 
            DateTime lastWeek = DateTime.UtcNow.AddDays(-7);

            return await _context.ViewedBooks.
            Where(v => v.ViewDate >= lastWeek).
            GroupBy(v => v.BookID).
            OrderByDescending(g => g.Count()).
            Take(30).
            Select(g => g.First().Book).
            ToListAsync();


        }
        public async Task<List<Book>> GetPopularAllTime()
        {
            return await _context.Books.
            OrderByDescending(b => b.ViewedBooks.Count()).
            Take(30).
            ToListAsync();

        }

        public async Task AddBookView(int BookID, int accountid)
        {
            Book book = await bookDetailsRepo.GetBookById(BookID);
            if (book == null) return;
            Account account = await accountRepoe.GetAccountDataByID(accountid);
            if (account == null) return;
            DateTime viewdate = DateTime.Now;
            ViewedBook existingView = await CheckedIfAlreadyViewed(BookID, accountid);
            if (existingView != null)
            {
                existingView.ViewDate = viewdate;
                _context.ViewedBooks.Update(existingView);
            }
            else
            {

                ViewedBook bookview = new ViewedBook
                {
                    AccountID = accountid,
                    BookID = BookID,
                    ViewDate = viewdate,
                };

                _context.ViewedBooks.Add(bookview);
                _context.SaveChanges();
            }
        }
        public async Task<ViewedBook> CheckedIfAlreadyViewed(int BookID, int accountid)
        {
            return await _context.ViewedBooks
                     .FirstOrDefaultAsync(v => v.AccountID == accountid && v.BookID == BookID);
        }



    }
}