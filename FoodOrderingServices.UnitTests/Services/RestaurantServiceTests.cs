using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Core.Services;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Services
{
    public class RestaurantServiceTests
    {
        private readonly Mock<IRestaurantRepositary>    _mockRepo;
        private readonly RestaurantService              _service;

        public RestaurantServiceTests()
        {
            _mockRepo = new Mock<IRestaurantRepositary>();
            _service  = new RestaurantService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllRestaurentsAsync_ReturnsDtoList_WhenRestaurantsExist()
        {
            var restaurants = new List<Restaurant>
            {
                new() { RestaurantID = Guid.NewGuid(), RestaurantName = "Cafe A", Location = "NY", ContactNumber = "1234567890" }
            };
            _mockRepo.Setup(r => r.GetAllRestaurentsAsync()).ReturnsAsync(restaurants);

            var result = await _service.GetAllRestaurentsAsync();

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task GetAllRestaurentsAsync_ReturnsNull_WhenRepositoryReturnsNull()
        {
            _mockRepo.Setup(r => r.GetAllRestaurentsAsync())
                     .ReturnsAsync((IEnumerable<Restaurant>)null!);

            var result = await _service.GetAllRestaurentsAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRestaurentByIdAsync_ReturnsDto_WhenFound()
        {
            var id         = Guid.NewGuid();
            var restaurant = new Restaurant { RestaurantID = id, RestaurantName = "Cafe A", Location = "NY", ContactNumber = "0987654321" };

            _mockRepo.Setup(r => r.GetRestaurentByIdAsync(id)).ReturnsAsync(restaurant);

            var result = await _service.GetRestaurentByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result!.RestaurantID);
        }

        [Fact]
        public async Task GetRestaurentByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetRestaurentByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Restaurant?)null);

            var result = await _service.GetRestaurentByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task AddRestaurentAsync_ReturnsRestaurant_WhenAddedSuccessfully()
        {
            var request    = BuildRequest();
            var restaurant = new Restaurant { RestaurantID = Guid.NewGuid(), RestaurantName = request.RestaurantName };

            _mockRepo.Setup(r => r.AddRestaurentAsync(request)).ReturnsAsync(restaurant);

            var result = await _service.AddRestaurentAsync(request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddRestaurentAsync_ReturnsNull_WhenDuplicate()
        {
            var request = BuildRequest();
            _mockRepo.Setup(r => r.AddRestaurentAsync(request)).ReturnsAsync((Restaurant?)null);

            var result = await _service.AddRestaurentAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateRestaurentAsync_ReturnsRestaurant_WhenUpdated()
        {
            var request    = BuildRequest();
            var restaurant = new Restaurant { RestaurantName = request.RestaurantName };

            _mockRepo.Setup(r => r.UpdateRestaurentAsync(request)).ReturnsAsync(restaurant);

            var result = await _service.UpdateRestaurentAsync(request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteRestaurentAsync_ReturnsTrue_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.DeleteRestaurentAsync(id)).ReturnsAsync(true);

            var result = await _service.DeleteRestaurentAsync(id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRestaurentAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.DeleteRestaurentAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _service.DeleteRestaurentAsync(Guid.NewGuid());

            Assert.False(result);
        }

        private static AddRestaurantRequest BuildRequest() => new()
        {
            RestaurantName = "Test Cafe",
            Location       = "London",
            ContactNumber  = "0123456789"
        };
    }
}