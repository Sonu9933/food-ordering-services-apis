using FoodOrderingServices.API.Controllers;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Controllers
{
    public class RestaurantControllerTests
    {
        private readonly Mock<IRestaurantService>           _mockService;
        private readonly Mock<ILogger<RestaurantController>> _mockLogger;
        private readonly RestaurantController               _controller;

        public RestaurantControllerTests()
        {
            _mockService = new Mock<IRestaurantService>();
            _mockLogger  = new Mock<ILogger<RestaurantController>>();
            _controller  = new RestaurantController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllRestaurentAsync_ReturnsOk_WithRestaurantList()
        {
            var restaurants = new List<RestaurantDetailDTO>
            {
                new() { RestaurantID = Guid.NewGuid(), RestaurantName = "Pizza Place",
                        Location = "NY", ContactNumber = "1234567890" }
            };
            _mockService.Setup(s => s.GetAllRestaurentsAsync()).ReturnsAsync(restaurants);

            var result = await _controller.GetAllRestaurentAsync();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllRestaurentAsync_Returns500_OnException()
        {
            _mockService.Setup(s => s.GetAllRestaurentsAsync()).ThrowsAsync(new Exception("DB error"));

            var result       = await _controller.GetAllRestaurentAsync();
            var statusResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task RestaurantRegistrationAsync_ReturnsOk_WhenRegistered()
        {
            var request    = BuildRequest();
            var restaurant = new Restaurant { RestaurantID = Guid.NewGuid(), RestaurantName = request.RestaurantName };

            _mockService.Setup(s => s.AddRestaurentAsync(request)).ReturnsAsync(restaurant);

            var result = await _controller.RestaurantRegistrationAsync(request);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task RestaurantRegistrationAsync_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            var request = BuildRequest();
            _mockService.Setup(s => s.AddRestaurentAsync(request)).ReturnsAsync((Restaurant?)null);

            var result = await _controller.RestaurantRegistrationAsync(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task RestaurantRegistrationAsync_Returns500_OnException()
        {
            var request = BuildRequest();
            _mockService.Setup(s => s.AddRestaurentAsync(request)).ThrowsAsync(new Exception("error"));

            var result       = await _controller.RestaurantRegistrationAsync(request);
            var statusResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateRestaurantDetailsAsync_ReturnsOk_WhenUpdated()
        {
            var request    = BuildRequest();
            var restaurant = new Restaurant { RestaurantName = request.RestaurantName };

            _mockService.Setup(s => s.UpdateRestaurentAsync(request)).ReturnsAsync(restaurant);

            var result = await _controller.UpdateRestaurantDetailsAsync(request);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateRestaurantDetailsAsync_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            var request = BuildRequest();
            _mockService.Setup(s => s.UpdateRestaurentAsync(request)).ReturnsAsync((Restaurant?)null);

            var result = await _controller.UpdateRestaurantDetailsAsync(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteRestaurentByIdAsync_ReturnsOk_WhenDeleteSucceeds()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteRestaurentAsync(id)).ReturnsAsync(true);

            var result = await _controller.DeleteRestaurentByIdAsync(id);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteRestaurentByIdAsync_ReturnsOk_WhenDeleteFails()
        {
            // Controller returns Ok either way, just different messages
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteRestaurentAsync(id)).ReturnsAsync(false);

            var result = await _controller.DeleteRestaurentByIdAsync(id);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        private static AddRestaurantRequest BuildRequest() => new()
        {
            RestaurantName = "Test Cafe",
            Location       = "London",
            ContactNumber  = "0123456789"
        };
    }
}