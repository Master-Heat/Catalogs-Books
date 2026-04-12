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
        private readonly CatalogsBooksContext _context;
        // Initializing the hasher directly as seen in your example
        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        public AccountsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Accounts
        /// Retrieves each row in the table
        /// </summary>
        [HttpGet]
        public IActionResult DisplayAllUsers()
        {
            var accounts = _context.Accounts.ToList();
            return Ok(accounts);
        }

        /// <summary>
        /// POST: api/Accounts
        /// Adds one row to the DB with a hashed password using Form Data
        /// </summary>
        [HttpPost]
        public IActionResult CreateAccount([FromForm] string username, [FromForm] string email, [FromForm] string password, [FromForm] string permissionLevel)
        {
            // Simple validation for required fields
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(permissionLevel))
            {
                return BadRequest(new { message = "Email, Password, and Permission Level are required." });
            }

            var account = new Account
            {
                UserName = username,
                Email = email,
                IsAdmin = false
            };

            // Hash the password using Microsoft.Extensions.Identity.Core
            account.PasswordHash = _passwordHasher.HashPassword(account, password);

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountID }, account);
        }

        /// <summary>
        /// GET: api/Accounts/{id}
        /// Retrieves a specific account by ID
        /// </summary>
        [HttpGet("{id}")]
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