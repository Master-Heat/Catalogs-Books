using System.Composition.Hosting.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        protected string GetAccountRole()
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



        [HttpGet("byid/{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetInfo(int id)
        {
            int IdFromToken = GetUserId();
            string roleClaimed = GetAccountRole();
            if (IdFromToken == 0 ||
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (IdFromToken != id && roleClaimed != "Admin")
            {
                return Forbid();
            }

            UserAccountDTO accountdata = await accountFactory.GetAccountDataByID(id);
            if (accountdata == null) return NotFound();
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

        [HttpGet("byemail/{email}")]
        [Authorize]
        public async Task<ActionResult> GetUserInfoFromEmail(string email)
        {
            string claimedRole = GetAccountRole();
            if (string.IsNullOrWhiteSpace(claimedRole))
            {
                return Forbid();
            }
            if (claimedRole == "Admin" || claimedRole == "AI")
            {
                AccountInfoDTO accountInfo = await accountFactory.GetAccountDataByEmil(email);
                if (accountInfo == null) return NotFound();
                return Ok(accountInfo);
            }
            return Unauthorized();

        }
        [HttpPut("changerole/{id:int}")]
        [Authorize]
        public async Task<ActionResult> ChangeUserRole(int id, [FromBody] string newRole)
        {
            int IdFromToken = GetUserId();
            string roleClaimed = GetAccountRole();
            if (IdFromToken == 0 ||
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (roleClaimed != "Admin")
            {
                return Forbid();
            }
            bool result = await accountFactory.ModifyAccountRole(id, newRole);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("changestate/{id:int}")]
        [Authorize]
        public async Task<ActionResult> ChangeUserState(int id, [FromBody] string newState)
        {
            int IdFromToken = GetUserId();
            string roleClaimed = GetAccountRole();
            if (IdFromToken == 0 ||
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (roleClaimed != "Admin")
            {
                return Forbid();
            }
            bool result = await accountFactory.ModifyAccountState(id, newState);
            if (!result) return NotFound();
            return NoContent();
        }

        // [HttpGet("viewedbooks{accountId:int}")]
        // [Authorize]
        // public async Task<ActionResult> GetUserViewedBooks(int accountId)
        // {
        //     int IdFromToken = GetUserId();
        //     string roleClaimed = GetAccountRole();
        //     if (IdFromToken == 0 ||
        //         string.IsNullOrWhiteSpace(roleClaimed))
        //     {
        //         return Unauthorized();
        //     }
        //     if (IdFromToken != accountId && roleClaimed != "Admin")
        //     {
        //         return Forbid();
        //     }
        //     if (roleClaimed == "Admin" || roleClaimed == "AI")
        //     {
        //         List<BookCardDTO> viewedBooks = await accountFactory.GetUserViewedBooks(IdFromToken);
        //         if (viewedBooks == null || viewedBooks.Count == 0) return NotFound();
        //         return Ok(viewedBooks);
        //     }
        //     return Forbid();

        // }




        [HttpGet("viewedbooks")]
        [Authorize]
        public async Task<ActionResult> GetUserViewedBooksFromToken()
        {
            int IdFromToken = GetUserId();
            if (IdFromToken == 0)
            {
                return Unauthorized();
            }

            List<BookCardDTO> viewedBooks = await accountFactory.GetUserViewedBooks(IdFromToken);
            if (viewedBooks == null || viewedBooks.Count == 0) return NotFound();
            return Ok(viewedBooks);

        }

        [HttpDelete("removeaccount/{id:int}")]
        [Authorize]
        public async Task<ActionResult> RemoveAccount(int id)
        {

            string roleClaimed = GetAccountRole();
            if (
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (roleClaimed != "Admin")
            {
                return Forbid();
            }
            bool result = await accountFactory.RemoveAccount(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}