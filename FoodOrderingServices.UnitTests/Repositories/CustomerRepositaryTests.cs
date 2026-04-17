using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Infrastructure.Data;
using FoodOrderingServices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CustomerEntity = FoodOrderingServices.Core.Entity.Customer;

namespace FoodOrderingServices.UnitTests.Repositories
{
    public class CustomerRepositaryTests : IDisposable
    {
        private readonly ApplicationDbContext       _context;
        private readonly Mock<ICacheRepository>    _mockCache;
        private readonly Mock<ILogger<CustomerRepositary>> _mockLogger;
        private readonly CustomerRepositary        _repository;

        public CustomerRepositaryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"CustomerDb_{Guid.NewGuid():N}")
                .Options;

            _context     = new ApplicationDbContext(options);
            _mockCache   = new Mock<ICacheRepository>();
            _mockLogger  = new Mock<ILogger<CustomerRepositary>>();

            // Default: cache miss for all keys
            _mockCache
                .Setup(c => c.GetAsync<CustomerEntity>(It.IsAny<string>()))
                .ReturnsAsync((CustomerEntity?)null);

            // Allow SetAsync calls without throwing
            _mockCache
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CustomerEntity>(),
                    It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Allow RemoveAsync calls without throwing
            _mockCache
                .Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Allow RefreshAsync calls without throwing
            _mockCache
                .Setup(c => c.RefreshAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Allow ExistsAsync calls without throwing
            _mockCache
                .Setup(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _repository = new CustomerRepositary(_context, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegisterCustomerAsync_ReturnsCustomer_WhenEmailIsNew()
        {
            var result = await _repository.RegisterCustomerAsync("Alice", "alice@test.com", "password");

            Assert.NotNull(result);
            Assert.Equal("alice@test.com", result!.Email);
        }

        [Fact]
        public async Task RegisterCustomerAsync_HashesPassword()
        {
            var result = await _repository.RegisterCustomerAsync("Alice", "alice@test.com", "password");

            Assert.True(BCrypt.Net.BCrypt.Verify("password", result!.PasswordHash));
        }

        [Fact]
        public async Task RegisterCustomerAsync_PersistsCustomerInDatabase()
        {
            await _repository.RegisterCustomerAsync("Alice", "alice@test.com", "password");

            Assert.Equal(1, await _context.Customers.CountAsync());
        }

        [Fact]
        public async Task RegisterCustomerAsync_ReturnsNull_WhenEmailAlreadyExists()
        {
            await SeedCustomerAsync("alice@test.com");

            var result = await _repository.RegisterCustomerAsync("Alice2", "alice@test.com", "pass2");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginCustomerAsync_ReturnsCustomer_WhenEmailExistsInDb()
        {
            await SeedCustomerAsync("alice@test.com");

            // Login with correct password
            var result = await _repository.LoginCustomerAsync("alice@test.com", "password");

            Assert.NotNull(result);
            Assert.Equal("alice@test.com", result!.Email);
        }

        [Fact]
        public async Task LoginCustomerAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            var result = await _repository.LoginCustomerAsync("nobody@test.com", "pass");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginCustomerAsync_ReturnsNull_WhenPasswordIsIncorrect()
        {
            await SeedCustomerAsync("alice@test.com");

            var result = await _repository.LoginCustomerAsync("alice@test.com", "wrongpassword");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginCustomerAsync_StoresCustomerInCache_AfterDbLookup()
        {
            await SeedCustomerAsync("alice@test.com");

            await _repository.LoginCustomerAsync("alice@test.com", "password");

            // Verify that SetAsync was called with the correct email key
            _mockCache.Verify(c => c.SetAsync(
                "alice@test.com",
                It.IsAny<CustomerEntity>(),
                It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task LoginCustomerAsync_RetrievesCustomerFromCache_OnSecondCall()
        {
            await SeedCustomerAsync("alice@test.com");

            // First login - retrieves from DB and caches
            var result1 = await _repository.LoginCustomerAsync("alice@test.com", "password");
            Assert.NotNull(result1);

            // Set up mock to return cached customer on second call
            var cachedCustomer = new CustomerEntity
            {
                CustomerId = result1!.CustomerId,
                CustomerName = result1.CustomerName,
                Email = result1.Email,
                PasswordHash = result1.PasswordHash,
                CreatedAt = result1.CreatedAt,
                UpdatedAt = result1.UpdatedAt
            };

            _mockCache
                .Setup(c => c.GetAsync<CustomerEntity>("alice@test.com"))
                .ReturnsAsync(cachedCustomer);

            // Second login - should retrieve from cache
            var result2 = await _repository.LoginCustomerAsync("alice@test.com", "password");

            Assert.NotNull(result2);
            Assert.Equal("alice@test.com", result2!.Email);
            // Verify GetAsync was called (cache lookup)
            _mockCache.Verify(c => c.GetAsync<CustomerEntity>("alice@test.com"), Times.AtLeastOnce);
        }

        private async Task<CustomerEntity> SeedCustomerAsync(string email)
        {
            var customer = new CustomerEntity
            {
                CustomerId   = Guid.NewGuid(),
                CustomerName = "Alice",
                Email        = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            return customer;
        }

        public void Dispose() => _context.Dispose();
    }
}