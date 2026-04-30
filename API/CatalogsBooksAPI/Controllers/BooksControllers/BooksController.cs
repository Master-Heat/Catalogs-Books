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
using Humanizer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;



namespace CatalogsBooksAPI.Controllers.AccountControllers
{
    [Route("api/[controller]/Authorized")]
    [ApiController]
    public class BooksController : ControllerBase
    {




        BookDetailsFactory bookDetailsFactory;
        AccountFactory accountFactory;

        public BooksController(BookDetailsFactory bookDetailsFactory, AccountFactory accountFactory)
        {
            this.bookDetailsFactory = bookDetailsFactory;
            this.accountFactory = accountFactory;
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookDetials(int id)
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
            UserAccountDTO accountFromTokenID = await accountFactory.GetAccountDataByID(int.Parse(IdFromToken));

            if (accountFromTokenID == null)
            {
                return Forbid();
            }

            BookDetailsDTO bookDetails = await bookDetailsFactory.GetBookDetails(id);
            return Ok(bookDetails);

        }






    }

}