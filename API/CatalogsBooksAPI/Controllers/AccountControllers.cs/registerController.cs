using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
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


        AccountRepo accountRepo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        private readonly IAccountFactory _accountFactory;

        public RegisterController(AccountRepo accountRepo, IAccountFactory accountFactory)
        {
            this.accountRepo = accountRepo;
            _accountFactory = accountFactory;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterDTO registerDto)
        {
            try
            {



                var newAccount = _accountFactory.CreateFromRegisterDTO(registerDto);

                await accountRepo.AddAccount(newAccount);




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