using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CatalogsBooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        CatalogsBooksContext _context;

        public AccountsController(CatalogsBooksContext context)
        {
            _context = context;
        }
        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        /// <summary>
        /// GET: api/accounts
        /// Retrieves all accounts from the database
        /// </summary>
        /// <returns>List of all accounts</returns>


        //[HttpGet]
        //[ProducesResponseType(typeof(List<Account>), 200)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult<List<Account>>> GetAllAccounts()
        //{
        //    try
        //    {
        //        var accounts = await _context.Accounts.ToListAsync();
        //        return Ok(accounts);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error retrieving accounts", error = ex.Message });
        //    }
        //}
        [HttpGet]
        public IActionResult DisplayAllUsers()
        {
            List<Account> accounts =
             _context.Accounts.ToList();
            return Ok(accounts);
        }

        /// <summary>
        /// POST: api/accounts
        /// Adds a new account to the database
        /// </summary>
        /// <param name="account">Account details to create</param>
        /// <returns>Created account</returns>
        // [HttpPost]
        // //  [ProducesResponseType(typeof(Account), 201)]
        // //  [ProducesResponseType(400)]
        // //  [ProducesResponseType(500)]
        // public IActionResult CreateAccount(Account account)
        // {
        // 
        // if (!ModelState.IsValid)
        // {
        // return BadRequest(ModelState);
        // }
        // 
        // if (string.IsNullOrWhiteSpace(account.Email))
        // {
        // return BadRequest(new { message = "Email is required" });
        // }
        // 
        // if (string.IsNullOrWhiteSpace(account.PermissionLevel))
        // {
        // return BadRequest(new { message = "Permission level is required" });
        // }
        // 
        // _context.Accounts.Add(account);
        // _context.SaveChanges();
        // 
        // return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountID }, account);
        // 
        // }
        // 
        // 
        [HttpPost]
        public IActionResult CreateAccount(string username, string email, string password, string permissionLevel)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            if (string.IsNullOrWhiteSpace(permissionLevel))
            {
                return BadRequest(new { message = "Permission level is required" });
            }

            var account = new Account
            {
                UserName = username,
                Email = email,
                PermissionLevel = permissionLevel
            };

            // Hash the password
            account.PasswordHash = _passwordHasher.HashPassword(account, password);

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountID }, account);
        }


        /// <summary>
        /// GET: api/accounts/{id}
        /// Retrieves a specific account by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountID == id);

            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found" });
            }

            return Ok(account);
        }
    }
}