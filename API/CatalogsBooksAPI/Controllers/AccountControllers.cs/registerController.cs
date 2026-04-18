using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CatalogsBooksAPI.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {

        private readonly CatalogsBooksContext _context;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        private readonly IAccountFactory _accountFactory;

        public RegisterController(CatalogsBooksContext context, IAccountFactory accountFactory)
        {
            _context = context;
            _accountFactory = accountFactory;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterDTO registerDto)
        {
            try
            {
                var existing = await _accountFactory.FindAccountByEmailAsync(registerDto.Email);
                if (existing != null)
                {
                    return BadRequest(new { message = "Email is already in use." });
                }


                var newAccount = _accountFactory.CreateFromRegisterDTO(registerDto);


                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();


                return Ok(new { message = $"Registration successful. Welcome, {newAccount.UserName}!" });
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An unexpected error occurred on the server." });
            }
        }
    }

}