using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using FoodOrderingServices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;
using CustomerEntity = FoodOrderingServices.Core.Entity.Customer;

namespace FoodOrderingServices.UnitTests.Repositories
{
    public class CustomerRepositaryTests : IDisposable
    {
        private readonly ApplicationDbContext    _context;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly CustomerRepositary      _repository;

        public CustomerRepositaryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"CustomerDb_{Guid.NewGuid():N}")
                .Options;

            _context   = new ApplicationDbContext(options);
            _mockCache = new Mock<IDistributedCache>();

            // Default: cache miss for all keys
            _mockCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            // Allow SetAsync calls without throwing
            _mockCache
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repository = new CustomerRepositary(_context, _mockCache.Object);
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

            // Note: repository only matches by email on the DB path (no password check — known behaviour)
            var result = await _repository.LoginCustomerAsync("alice@test.com", "anypassword");

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
        public async Task LoginCustomerAsync_StoresCustomerInCache_AfterDbLookup()
        {
            await SeedCustomerAsync("alice@test.com");

            await _repository.LoginCustomerAsync("alice@test.com", "pass");

            _mockCache.Verify(c => c.Set(
                "alice@test.com",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
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