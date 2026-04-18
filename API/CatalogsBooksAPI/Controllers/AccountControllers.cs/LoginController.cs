using System.Composition.Hosting.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace CatalogsBooksAPI.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {

        private readonly CatalogsBooksContext _context;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        private readonly IAccountFactory _accountFactory;

        public LogInController(CatalogsBooksContext context)
        {
            _context = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountloginDTO Accountdata)

        {
            Account existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email.ToLower() == Accountdata.Email.ToLower());

            if (existingAccount == null)
            {
                return Unauthorized("Invalid Email or password");
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
                existingAccount,
                existingAccount.PasswordHash,
                Accountdata.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid Email or Password");

            }




            return Ok(new { message = $"Welcome Back, {existingAccount.UserName}!" });
        }


    }

}