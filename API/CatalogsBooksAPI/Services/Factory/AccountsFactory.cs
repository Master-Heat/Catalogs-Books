
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Services.Factories
{






    public class AccountFactory
    {
        AccountRepo repo;

        // private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        private readonly IPasswordHasher<Account> _passwordHasher;
        public AccountFactory(AccountRepo repo, IPasswordHasher<Account> passwordHasher)
        {
            this.repo = repo;
            _passwordHasher = passwordHasher;
        }

        // 1. Search Logic: Used to check if they should redirect to Login


        // 2. Creation Logic
        public async Task<Account> CreateFromRegisterDTO(AccountRegisterDTO dto)
        {
            await ValidateRegisterDTO(dto);

            Account account = new Account
            {
                UserName = dto.UserName,
                Email = dto.Email,

                Role = "User" // Defaulting to false as requested
            };
            account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);
            return account;
        }
        public async Task<UserAccountDTO> GetAccountDataByID(int id)
        {
            Account dbaccount = await repo.GetAccountDataByID(id);
            UserAccountDTO account = new UserAccountDTO
            {
                UserName = dbaccount.UserName,
                Email = dbaccount.Email
            };
            return account;
        }
        public async Task<AccountInfoDTO> GetAccountDataByEmil(string Email)
        {
            Account dbaccount = await repo.GetAccountDataByEmail(Email);
            AccountInfoDTO account = new AccountInfoDTO
            {
                AccountID = dbaccount.AccountID,
                UserName = dbaccount.UserName,
                Role = dbaccount.Role,
                AccountState = dbaccount.AccountState
            };
            return account;
        }

        public async Task<bool> ModifyAccountRole(int id, string newRole)
        {
            return await repo.ModifyAccountRole(id, newRole);
        }


        private async Task ValidateRegisterDTO(AccountRegisterDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Registration data is missing.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");

            Account existingAccount = await repo.GetAccountDataByEmail(dto.Email);
            if (existingAccount != null)
            {
                throw new ArgumentException("Email is reserved for another account");

            }
        }
    }
}