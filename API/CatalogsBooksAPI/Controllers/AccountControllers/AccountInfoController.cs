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
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;



namespace CatalogsBooksAPI.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInfoController : ControllerBase
    {




        AccountFactory accountFactory;
        HomePageFactory homePageFactory;

        public AccountInfoController(AccountFactory accountFactory,
                                    HomePageFactory homePageFactory)
        {
            this.accountFactory = accountFactory;
            this.homePageFactory = homePageFactory;
        }

        [NonAction] // Tells ASP.NET this is not an API endpoint
        protected int GetUserId()
        {
            var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(id, out int result) ? result : 0;
        }

        [NonAction]
        protected string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                   ?? User.FindFirst("role")?.Value
                   ?? string.Empty;
        }

        [NonAction]
        protected string GetUserEmail()
        {
            return User.FindFirst(JwtRegisteredClaimNames.Name)?.Value
              ?? User.FindFirst(ClaimTypes.Name)?.Value
              ?? User.FindFirst("name")?.Value
              ?? string.Empty;
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInfo(int id)
        {
            int IdFromToken = GetUserId();
            string roleClaimed = GetUserRole();
            if (IdFromToken == 0 ||
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (IdFromToken != id && roleClaimed != "Admin")
            {
                return Forbid();
            }

            UserAccountDTO accountdata = await accountFactory.GetAccountDataByID(IdFromToken);
            return Ok(accountdata);



        }
        [HttpGet("homepage")]
        [Authorize]
        public async Task<ActionResult> GetUserHomePage()
        {
            string email = GetUserEmail();
            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized();
            }
            HomeDashboardDTO homeDashboard = await homePageFactory.GenerateHomeData(email);
            return Ok(homeDashboard);
        }






    }

}