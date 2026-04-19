
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Services.Factories
{




    public interface IAccountFactory
    {

        Account CreateFromRegisterDTO(AccountRegisterDTO dto);
    }



    public class AccountFactory : IAccountFactory
    {
        AccountRepo repo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        public AccountFactory(AccountRepo repo)
        {
            this.repo = repo;
        }

        // 1. Search Logic: Used to check if they should redirect to Login


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
            if (repo.GetAccountDataByEmail(dto.Email) != null)
            {
                throw new ArgumentException("Email is reserved for another account");

            }
        }
    }
}