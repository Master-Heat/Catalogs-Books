
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Services.Factories
{




    public interface IAccountFactory
    {
        Task<Account> FindAccountByEmailAsync(string email);
        Account CreateFromRegisterDTO(AccountRegisterDTO dto, string hashedPassword);
    }



    public class AccountFactory : IAccountFactory
    {
        private readonly CatalogsBooksContext _context;

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
        public Account CreateFromRegisterDTO(AccountRegisterDTO dto, string hashedPassword)
        {
            ValidateRegisterDTO(dto);

            return new Account
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                IsAdmin = false // Defaulting to false as requested
            };
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