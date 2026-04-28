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
    public class AccountInfoController : ControllerBase
    {




        AccountFactory accountFactory;

        public AccountInfoController(AccountFactory accountFactory)
        {
            this.accountFactory = accountFactory;
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInfo(int id)
        {
            string IdFromToken = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string roleClaimed = User.FindFirst(ClaimTypes.Role)?.Value
                       ?? User.FindFirst("role")?.Value;
            if (string.IsNullOrWhiteSpace(IdFromToken) ||
                string.IsNullOrWhiteSpace(roleClaimed))
            {
                return Unauthorized();
            }
            if (IdFromToken != id.ToString() && roleClaimed != "Admin")
            {
                return Forbid();
            }

            UserAccountDTO accountdata = await accountFactory.GetAccountDataByID(int.Parse(IdFromToken));
            return Ok(accountdata);

        }






    }

}