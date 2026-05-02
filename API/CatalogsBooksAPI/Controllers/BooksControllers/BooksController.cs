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
        BookFactory bookFactory;
        BookDetailsRepo bookDetailsRepo;
        public BooksController(BookDetailsFactory bookDetailsFactory,
                                AccountFactory accountFactory,
                                BookviewsRepo bookviews,
                                BooksRecsCardListFactory bookCardListFactory,
                                BookFactory bookFactory,
                                BookDetailsRepo bookDetailsRepo
                                )
        {
            this.bookDetailsFactory = bookDetailsFactory;
            this.accountFactory = accountFactory;
            this.bookviews = bookviews;
            this.bookCardListFactory = bookCardListFactory;
            this.bookFactory = bookFactory;
            this.bookDetailsRepo = bookDetailsRepo;
        }


        [NonAction]
        protected string GetAccountRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                   ?? User.FindFirst("role")?.Value
                   ?? string.Empty;
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

        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult> AddBook(CreateBookDTO bookDTO)
        {
            string roleFromToken = GetAccountRole();
            if (roleFromToken == "Admin")
            {

                try
                {
                    // 1. Use the Factory to validate and map the DTO
                    var book = await bookFactory.CreateFromDTO(bookDTO);

                    // 2. Logic Check: If factory returns a book with an ID, it already exists
                    if (book.BookID > 0)
                    {
                        return Conflict(new { message = "A book with this title already exists." });
                    }

                    // 3. Save to database via Repository
                    // Note: You may need to add an 'AddBook' method to your BookDetailsRepo
                    await bookDetailsRepo.AddBook(book);

                    return CreatedAtAction(nameof(AddBook), new { id = book.BookID }, book);
                }
                catch (ArgumentException ex)
                {
                    // Catch the specific validation errors from your Factory
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception)
                {
                    return StatusCode(500, "An error occurred while saving the book.");
                }
            }
            return Forbid();

        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllBooks()
        {
            string RoleFromToken = GetAccountRole();
            if (RoleFromToken == "Admin" || RoleFromToken == "AI")
            {


                List<BookCardDTO> allBooks = await bookFactory.GetAllBooks();
                return Ok(allBooks);
            }
            else return Forbid();
        }
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> AlterBook(AlterBookDTO book)
        {
            string RoleFromToken = GetAccountRole();
            if (RoleFromToken == "Admin")
            {
                bool result = await bookFactory.alterExistingBook(book);
                if (result) return Ok(book);
                else return NotFound(new { message = "No book found with the provided ID." });
            }
            else return Forbid();
        }
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteBook(int id)
        {
            string RoleFromToken = GetAccountRole();
            if (RoleFromToken == "Admin")
            {
                bool result = await bookDetailsRepo.DeleteBook(id);
                if (result) return Ok();
                else return NotFound(new { message = "No book found with the provided ID." });
            }
            else return Forbid();
        }
    }




}



