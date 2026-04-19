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



    }
}