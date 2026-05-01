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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;



namespace CatalogsBooksAPI.Controllers.BooksControllers
{
    [Route("api/[controller]/Authorized")]
    [ApiController]
    public class BooksController : ControllerBase
    {




        BookDetailsFactory bookDetailsFactory;
        AccountFactory accountFactory;

        BookviewsRepo bookviews;
        BooksRecsCardListFactory bookCardListFactory;
        public BooksController(BookDetailsFactory bookDetailsFactory,
                                AccountFactory accountFactory,
                                BookviewsRepo bookviews,
                                BooksRecsCardListFactory bookCardListFactory)
        {
            this.bookDetailsFactory = bookDetailsFactory;
            this.accountFactory = accountFactory;
            this.bookviews = bookviews;
            this.bookCardListFactory = bookCardListFactory;
        }



        [HttpGet("{id:int}")]
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
            await bookviews.AddBookView(id, int.Parse(IdFromToken));
            return Ok(bookDetails);


        }


        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult> Search([FromQuery] string keyword)
        {
            List<BookCardDTO> SearchResult = await bookCardListFactory.SmartSearch(keyword);
            return Ok(SearchResult);
        }


    }

}