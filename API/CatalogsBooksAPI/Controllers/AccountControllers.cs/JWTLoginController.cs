using System.Composition.Hosting.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
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

        AccountRepo accountRepo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        // private readonly IAccountFactory _accountFactory;
        HomePageFactory homePageFactory;
        private readonly Authentication _authentication;

        public LogInController(AccountRepo accountRepo, HomePageFactory homePageFactory, Authentication authentication)
        {
            this.accountRepo = accountRepo;
            this.homePageFactory = homePageFactory;
            _authentication = authentication;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccountloginDTO claimedAccount)

        {
            string JWTToken = await _authentication.Authenticate(claimedAccount);
            if (JWTToken == null)
            {
                return Unauthorized("Invalid User Name Or Password");
            }

            HomeDashboardDTO dashboardData = await homePageFactory.GenerateHomeData(JWTToken, claimedAccount.Email);

            return Ok(dashboardData);
        }






    }

}