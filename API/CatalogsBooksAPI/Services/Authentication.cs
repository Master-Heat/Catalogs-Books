using System.Composition.Hosting.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
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

        private readonly CatalogsBooksContext _context;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        private readonly IAccountFactory _accountFactory;
        private readonly IConfiguration _config;

        public Authentication(CatalogsBooksContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }


        public async Task<string> Authenticate(AccountloginDTO claimedAccount)
        {
            if (string.IsNullOrWhiteSpace(claimedAccount.Email) || string.IsNullOrWhiteSpace(claimedAccount.Password))
            {
                return null;
            }
            Account dbaccount = await _context.Accounts
        .FirstOrDefaultAsync(a => a.Email.ToLower() == claimedAccount.Email.ToLower());
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
                    new Claim(ClaimTypes.Role, dbaccount.IsAdmin ? "Admin" : "User")
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