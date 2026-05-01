
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.DTOs.ListsDTOs;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogsBooksAPI.Controllers.BooksControllers
{
    [Authorize] // Ensures only logged-in users with a valid token can access
    [Route("api/[controller]/Authorized")]
    [ApiController]
    public class BookListsController : ControllerBase
    {
        private readonly ListsFactory _listsFactory;

        public BookListsController(ListsFactory listsFactory)
        {
            _listsFactory = listsFactory;
        }

        // Helper method to extract AccountID from the JWT Token
        private int GetUserIdFromToken()
        {
            // Make sure your Token was created using ClaimTypes.NameIdentifier or "sub"
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("AccountId");

            if (claim == null) return 0;

            return int.Parse(claim.Value);
        }

        [HttpGet("MyLists")]
        public async Task<IActionResult> GetMyLists()
        {
            int accountId = GetUserIdFromToken();
            var lists = await _listsFactory.GetAllUserListsWithBooks(accountId);
            return Ok(lists);
        }

        [HttpPost("CreateList")]
        public async Task<IActionResult> CreateList([FromBody] string listName)
        {
            int accountId = GetUserIdFromToken();
            try
            {
                await _listsFactory.CreateEmptyUserList(accountId, listName.Trim());
                return Ok(new { message = $"List '{listName.Trim()}' created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddBookToList")]
        public async Task<IActionResult> AddBook([FromBody] AddBookToListDTO request)
        {
            int accountId = GetUserIdFromToken();

            try
            {
                await _listsFactory.AddBookToList(accountId, request.ListID, request.BookID);
                return Ok(new { message = "Book added to list successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // FIX: Do not use Forbid(ex.Message). Use this instead:
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }
    }
}

