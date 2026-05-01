using System.Composition.Hosting.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace CatalogsBooksAPI.Services
{
    public class Authentication
    {


        AccountRepo accountRepo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        private readonly AccountFactory _accountFactory;
        private readonly IConfiguration _config;

        public Authentication(AccountRepo accountRepo, IConfiguration configuration)
        {
            this.accountRepo = accountRepo;
            _config = configuration;
        }


        public async Task<string> Authenticate(AccountloginDTO claimedAccount)
        {
            if (string.IsNullOrWhiteSpace(claimedAccount.Email) || string.IsNullOrWhiteSpace(claimedAccount.Password))
            {
                return null;
            }
            Account dbaccount = await accountRepo.GetAccountDataByEmail(claimedAccount.Email);
            if (dbaccount == null)
            {
                return null;
            }
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
                dbaccount,
                dbaccount.PasswordHash,
                claimedAccount.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                return null;

            }
            var key = _config["JWTConfig:Key"];

            var tokenDescrtor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim (JwtRegisteredClaimNames.Name,claimedAccount.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, dbaccount.AccountID.ToString()),
                    new Claim(ClaimTypes.Role, dbaccount.Role )
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature),

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescrtor);

            string AccessToken = tokenHandler.WriteToken(securityToken);

            return AccessToken;





        }


    }
}