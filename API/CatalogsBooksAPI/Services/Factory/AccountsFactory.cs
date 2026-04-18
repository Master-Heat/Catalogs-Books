
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Services.Factories
{




    public interface IAccountFactory
    {
        Task<Account> FindAccountByEmailAsync(string email);
        Account CreateFromRegisterDTO(AccountRegisterDTO dto);
    }



    public class AccountFactory : IAccountFactory
    {
        private readonly CatalogsBooksContext _context;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        public AccountFactory(CatalogsBooksContext context)
        {
            _context = context;
        }

        // 1. Search Logic: Used to check if they should redirect to Login
        public async Task<Account> FindAccountByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await _context.Accounts
                .AsQueryable()
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
        }

        // 2. Creation Logic
        public Account CreateFromRegisterDTO(AccountRegisterDTO dto)
        {
            ValidateRegisterDTO(dto);

            Account account = new Account
            {
                UserName = dto.UserName,
                Email = dto.Email,

                IsAdmin = false // Defaulting to false as requested
            };
            account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);
            return account;
        }

        private void ValidateRegisterDTO(AccountRegisterDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Registration data is missing.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");
        }
    }
}