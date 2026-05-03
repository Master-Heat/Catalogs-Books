using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CatalogsBooksAPI.Controllers.BooksControllers; // Ensure this matches your project
using CatalogsBooksAPI.DTOs.BooksDTOs; // Ensure this matches your project
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MyProject.Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetAllBooks_WhenAdmin_ReturnsOk()
        {


            var mockBookFactory = new Mock<BookFactory>(null!);




            // Mock the other dependencies using null! to satisfy constructors
            var mockDetailsFactory = new Mock<BookDetailsFactory>(null!, null!, null!);
            var mockAccountFactory = new Mock<AccountFactory>(null!, null!);
            var mockViewsRepo = new Mock<BookviewsRepo>(null!, null!, null!);
            var mockRecsFactory = new Mock<BooksRecsCardListFactory>(null!, null!);
            var mockDetailsRepo = new Mock<BookDetailsRepo>(null!);



            // 1. ARRANGE - Mocking all 6 dependencies
            // We use Mock<T> even for classes if the methods are virtual, 
            // but since you use classes, we create "empty" instances or mocks.


            // Set up the specific behavior for the factory function you shared
            var expectedBooks = new List<BookCardDTO> { new BookCardDTO { Title = "Test Book" } };
            mockBookFactory
                .Setup(f => f.GetAllBooks())
                .ReturnsAsync(expectedBooks);

            // Setup the mock to return our specific title
            mockBookFactory
                .Setup(f => f.GetAllBooks())
                .ReturnsAsync(expectedBooks);
            // Pass all 6 mocks into the constructor
            var controller = new BooksController(
                mockDetailsFactory.Object,
                mockAccountFactory.Object,
                mockViewsRepo.Object,
                mockRecsFactory.Object,
                mockBookFactory.Object,
                mockDetailsRepo.Object
            );

            // 2. ISOLATE GetAccountRole()
            // We simulate the User Claims so GetAccountRole() finds "Admin"
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // 3. ACT
            var result = await controller.GetAllBooks();

            // 4. ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedBooks, okResult.Value);
        }






    }
}