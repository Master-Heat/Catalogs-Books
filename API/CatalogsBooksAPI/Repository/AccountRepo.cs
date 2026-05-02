using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Repository
{
    public class AccountRepo
    {
        CatalogsBooksContext _context;

        public AccountRepo(CatalogsBooksContext context)
        {
            _context = context;
        }
        public async Task<Account> GetAccountDataByID(int id)
        {
            return await _context.Accounts.
            FirstOrDefaultAsync(a => a.AccountID == id);
        }

        public async Task<Account> GetAccountDataByEmail(string mail)
        {
            return await _context.Accounts.
            FirstOrDefaultAsync(a => a.Email == mail);
        }

        public async Task AddAccount(Account newAccount)
        {
            _context.Accounts.Add(newAccount);
            _context.SaveChanges();
        }
        public async Task<bool> ModifyAccountRole(int id, string newRole)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountID == id);
            if (account != null)
            {
                account.Role = newRole;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<bool> ModifyAccountState(int id, string newState)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountID == id);
            if (account != null)
            {
                account.AccountState = newState;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<Book>> GetUserViewedBooks(int accountId)
        {
            var viewedBooks = await _context.ViewedBooks
                .Where(vb => vb.AccountID == accountId)
                .Include(vb => vb.Book)
                .Select(vb => vb.Book)
                .ToListAsync();

            return viewedBooks;
        }
        public async Task<bool> removeAccount(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountID == id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}