
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CatalogsBooksAPI.DTOs.AuthorDTOs;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CatalogsBooksAPI.Controllers.AuthorController

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // This covers EVERY function in this controller
    public class AuthorController : ControllerBase
    {
        AuthorFactory _authorFactory;

        public AuthorController(AuthorFactory authorFactory)
        {
            _authorFactory = authorFactory;
        }
        [HttpGet("{authorName}")]
        public async Task<IActionResult> GetAuthorByName(string authorName)
        {
            var author = await _authorFactory.FindAuthorByNameAsync(authorName);
            if (author == null) return NotFound($"No author found with name '{authorName}'.");

            return Ok(author);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorFactory.GetAllAuthorsAsync();
            return Ok(authors);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDTO author)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authorFactory.CreateAuthorFromDTOAsync(author);
            return CreatedAtAction(nameof(GetAuthorByName), new { authorName = author.AuthorName }, author);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var success = await _authorFactory.DeleteAuthorAsync(id);
            if (!success) return NotFound($"No author found with ID '{id}'.");

            return NoContent();

        }
        [HttpGet("AuthorWithBooks/{authorId}")]
        public async Task<IActionResult> GetAuthorWithBooks(int authorId)
        {
            var authorWithBooks = await _authorFactory.GetAuthorWithBooksAsync(authorId);
            if (authorWithBooks == null) return NotFound($"No author found with ID '{authorId}'.");

            return Ok(authorWithBooks);
        }
        [HttpGet("AllAuthorsWithBooks")]
        public async Task<IActionResult> GetAllAuthorsWithBooks()
        {
            var authorsWithBooks = await _authorFactory.GetAllAuthorsWithBooksAsync();
            return Ok(authorsWithBooks);
        }

    }
}
