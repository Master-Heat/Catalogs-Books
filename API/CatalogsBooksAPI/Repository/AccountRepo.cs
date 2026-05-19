using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Validation;
namespace CatalogsBooksAPI.Repository
{
    public class AccountRepo
    {
        CatalogsBooksContext _context;
        private readonly IPasswordHasher<Account> _passwrodHasher;

        public AccountRepo(CatalogsBooksContext context, IPasswordHasher<Account> passwordHasher)
        {
            _context = context;
            _passwrodHasher = passwordHasher;
        }
        public async Task ValidateRegisterfromDTO(AccountRegisterDTO accountRegister)
        {
            if (accountRegister == null)
                throw new ArgumentNullException(nameof(accountRegister), "Registration data is missing.");

            if (string.IsNullOrWhiteSpace(accountRegister.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(accountRegister.Password))
                throw new ArgumentException("Password is required.");

            AccountInfoDTO existingAccount = await GetAccountDataByEmail(accountRegister.Email);
            if (existingAccount != null)
            {
                throw new ArgumentException("Email is reserved for another account");
            }
        }
        public async Task<Account> CreateFromRegisterDTO(AccountRegisterDTO accountRegister)
        {
            await ValidateRegisterfromDTO(accountRegister);
            Account newAccount = new Account
            {
                UserName = accountRegister.UserName,
                Email = accountRegister.Email,
                Role = "User",
                AccountState = "Active"
            };
            newAccount.PasswordHash = _passwrodHasher.HashPassword(newAccount, accountRegister.Password);
            _context.Accounts.Add(newAccount);
            _context.SaveChanges();
            return newAccount;
        }


        public async Task<UserAccountDTO> GetAccountDataByID(int id)
        {
            return await _context.Accounts.Where(a => a.AccountID == id)
            .Select(
                a => new UserAccountDTO
                {
                    UserName = a.UserName,
                    Email = a.Email,
                }
            ).FirstOrDefaultAsync();

        }

        public async Task<AccountInfoDTO> GetAccountDataByEmail(string mail)
        {
            return await _context.Accounts
                    .Where(a => a.Email == mail)
                    .Select(
                        a => new AccountInfoDTO
                        {
                            AccountID = a.AccountID,
                            UserName = a.UserName,
                            Role = a.Role,
                            AccountState = a.AccountState

                        }
                    ).FirstOrDefaultAsync();
        }
        public async Task<Account> GetAccountByEmail(string mail)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == mail);
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

        public async Task<List<BookCardDTO>> GetUserViewedBooks(int accountId)
        {
            List<BookCardDTO> viewedBooks = await _context.ViewedBooks
                .Where(vb => vb.AccountID == accountId && vb != null)
                .Include(vb => vb.Book)
                .Select(vb => new BookCardDTO
                {
                    BookID = vb.Book.BookID,
                    Title = vb.Book.Title,
                    Description = vb.Book.Description,
                    CoverImageLink = vb.Book.CoverImageLink,
                    CoverAlt = vb.Book.CoverAlt
                })
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