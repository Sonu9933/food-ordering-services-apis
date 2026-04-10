using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using FoodOrderingServices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodOrderingServices.UnitTests.Repositories
{
    public class RestaurantRepositaryTests : IDisposable
    {
        private readonly ApplicationDbContext   _context;
        private readonly RestaurantRepositary   _repository;

        public RestaurantRepositaryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"RestaurantDb_{Guid.NewGuid():N}")
                .Options;

            _context    = new ApplicationDbContext(options);
            _repository = new RestaurantRepositary(_context);
        }

        [Fact]
        public async Task AddRestaurentAsync_ReturnsRestaurant_WhenNameAndNumberAreUnique()
        {
            var request = BuildRequest("Cafe A", "1234567890");

            var result = await _repository.AddRestaurentAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Cafe A", result!.RestaurantName);
        }

        [Fact]
        public async Task AddRestaurentAsync_ReturnsNull_WhenDuplicateNameExists()
        {
            await SeedRestaurantAsync("Cafe A", "1111111111");

            var request = BuildRequest("Cafe A", "9999999999");
            var result  = await _repository.AddRestaurentAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddRestaurentAsync_ReturnsNull_WhenDuplicateContactExists()
        {
            await SeedRestaurantAsync("Cafe B", "1234567890");

            var request = BuildRequest("Cafe C", "1234567890");
            var result  = await _repository.AddRestaurentAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllRestaurentsAsync_ReturnsAllRestaurants()
        {
            await SeedRestaurantAsync("Cafe A", "1111111111");
            await SeedRestaurantAsync("Cafe B", "2222222222");

            var result = await _repository.GetAllRestaurentsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllRestaurentsAsync_ReturnsEmpty_WhenNoRestaurantsExist()
        {
            var result = await _repository.GetAllRestaurentsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRestaurentByIdAsync_ReturnsRestaurant_WhenExists()
        {
            var restaurant = await SeedRestaurantAsync("Cafe A", "1234567890");

            var result = await _repository.GetRestaurentByIdAsync(restaurant.RestaurantID);

            Assert.NotNull(result);
            Assert.Equal(restaurant.RestaurantID, result!.RestaurantID);
        }

        [Fact]
        public async Task GetRestaurentByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _repository.GetRestaurentByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        private async Task<Restaurant> SeedRestaurantAsync(string name, string phone)
        {
            var restaurant = new Restaurant
            {
                RestaurantID   = Guid.NewGuid(),
                RestaurantName = name,
                Location       = "London",
                ContactNumber  = phone,
                CreatedAt      = DateTime.UtcNow
            };
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        private static AddRestaurantRequest BuildRequest(string name, string phone) => new()
        {
            RestaurantName = name,
            Location       = "London",
            ContactNumber  = phone
        };

        public void Dispose() => _context.Dispose();
    }
}