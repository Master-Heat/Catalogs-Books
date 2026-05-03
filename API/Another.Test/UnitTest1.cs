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

namespace Another.Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task Search_WhenCalled_ReturnsOkWithResults()
        {
            // 1. ARRANGE - Mocking dependencies
            var mockRecsFactory = new Mock<BooksRecsCardListFactory>(null!, null!);

            // Setup remaining 5 mocks for the constructor
            var mockDetailsFactory = new Mock<BookDetailsFactory>(null!, null!, null!);
            var mockAccountFactory = new Mock<AccountFactory>(null!, null!);
            var mockViewsRepo = new Mock<BookviewsRepo>(null!, null!, null!);
            var mockBookFactory = new Mock<BookFactory>(null!);
            var mockDetailsRepo = new Mock<BookDetailsRepo>(null!);

            // Define the search behavior
            var keyword = "Fantasy";
            var expectedResults = new List<BookCardDTO>
    {
        new BookCardDTO { Title = "The Hobbit" }
    };

            mockRecsFactory
                .Setup(f => f.SmartSearch(keyword))
                .ReturnsAsync(expectedResults);

            var controller = new BooksController(
                mockDetailsFactory.Object,
                mockAccountFactory.Object,
                mockViewsRepo.Object,
                mockRecsFactory.Object, // This is the mock we are testing
                mockBookFactory.Object,
                mockDetailsRepo.Object
            );

            // 2. ISOLATE Identity (Required because of [Authorize])
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // 3. ACT
            var result = await controller.Search(keyword);

            // 4. ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResults, okResult.Value);
        }


    }
}