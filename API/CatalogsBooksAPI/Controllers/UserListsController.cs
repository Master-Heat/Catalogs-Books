using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogsBooksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserListsController : ControllerBase
    {
        private readonly CatalogsBooksContext _context;

        public UserListsController(CatalogsBooksContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/userlists
        /// Retrieves all user reading lists from the database
        /// </summary>
        /// <returns>List of all user lists</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserList>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UserList>>> GetAllUserLists()
        {
            try
            {
                var userLists = await _context.UserLists
                    .Include(ul => ul.Account)
                    .Include(ul => ul.BookLists)
                    .ToListAsync();

                return Ok(userLists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user lists", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/userlists
        /// Adds a new user reading list to the database
        /// </summary>
        /// <param name="userList">User list details to create</param>
        /// <returns>Created user list</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserList), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserList>> CreateUserList([FromBody] UserList userList)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (userList.AccountID <= 0)
                {
                    return BadRequest(new { message = "Valid AccountID is required" });
                }

                if (string.IsNullOrWhiteSpace(userList.ListName))
                {
                    return BadRequest(new { message = "List name is required" });
                }

                // Verify account exists
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountID == userList.AccountID);
                if (!accountExists)
                {
                    return BadRequest(new { message = "Invalid AccountID" });
                }

                _context.UserLists.Add(userList);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserListById), new { id = userList.ListID }, userList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user list", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/userlists/{id}
        /// Retrieves a specific user list by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserList), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserList>> GetUserListById(int id)
        {
            var userList = await _context.UserLists
                .Include(ul => ul.Account)
                .Include(ul => ul.BookLists)
                .FirstOrDefaultAsync(ul => ul.ListID == id);

            if (userList == null)
            {
                return NotFound(new { message = $"User list with ID {id} not found" });
            }

            return Ok(userList);
        }
    }
}