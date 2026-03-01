using System.Security.Cryptography.X509Certificates;
using CatalogsBooksAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace CatalogsBooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        CatalogsBooksContext _context;
    }
}