using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CatalogsBooksAPI.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {


        AccountRepo accountRepo;





        public RegisterController(AccountRepo accountRepo)
        {
            this.accountRepo = accountRepo;

        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterDTO registerDto)
        {
            try
            {



                Account newAccount = await accountRepo.CreateFromRegisterDTO(registerDto);






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