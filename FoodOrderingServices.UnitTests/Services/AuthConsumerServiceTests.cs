using ConsumerEnpoints.Services; 
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Customer;
using Moq;
using Xunit;
using CustomerEntity = FoodOrderingServices.Core.Entity.Customer;

namespace FoodOrderingServices.UnitTests.Services
{
    public class AuthConsumerServiceTests
    {
        private readonly Mock<ICustomerRepositary>  _mockRepo;
        private readonly Mock<IJwtTokenService>     _mockJwt;
        private readonly AuthConsumerService        _service;

        public AuthConsumerServiceTests()
        {
            _mockRepo = new Mock<ICustomerRepositary>();
            _mockJwt  = new Mock<IJwtTokenService>();
            _service  = new AuthConsumerService(_mockRepo.Object, _mockJwt.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsAuthResponse_WhenCredentialsAreValid()
        {
            var request  = new LoginCustomerRequest { Email = "user@test.com", Password = "pass" };
            var customer = BuildCustomer("user@test.com");

            _mockRepo.Setup(r => r.LoginCustomerAsync(request.Email, request.Password))
                     .ReturnsAsync(customer);
            _mockJwt.Setup(j => j.GenerateToken(customer)).Returns("jwt-token");

            var result = await _service.AuthenticateAsync(request);

            Assert.NotNull(result);
            Assert.Equal("jwt-token", result!.Token);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNull_WhenCustomerNotFound()
        {
            var request = new LoginCustomerRequest { Email = "none@test.com", Password = "pass" };
            _mockRepo.Setup(r => r.LoginCustomerAsync(request.Email, request.Password))
                     .ReturnsAsync((CustomerEntity?)null);

            var result = await _service.AuthenticateAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_CallsGenerateToken_WhenCustomerFound()
        {
            var request  = new LoginCustomerRequest { Email = "user@test.com", Password = "pass" };
            var customer = BuildCustomer("user@test.com");

            _mockRepo.Setup(r => r.LoginCustomerAsync(request.Email, request.Password))
                     .ReturnsAsync(customer);
            _mockJwt.Setup(j => j.GenerateToken(customer)).Returns("jwt-token");

            await _service.AuthenticateAsync(request);

            _mockJwt.Verify(j => j.GenerateToken(customer), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsCustomer_WhenRegistrationSucceeds()
        {
            var request  = new RegisterCustomerRequest { ConsumerName = "Alice", Email = "alice@test.com", Password = "pass" };
            var customer = BuildCustomer("alice@test.com");

            _mockRepo.Setup(r => r.RegisterCustomerAsync(request.ConsumerName, request.Email, request.Password))
                     .ReturnsAsync(customer);

            var result = await _service.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.Equal("alice@test.com", result!.Email);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsNull_WhenEmailAlreadyExists()
        {
            var request = new RegisterCustomerRequest { ConsumerName = "Alice", Email = "alice@test.com", Password = "pass" };
            _mockRepo.Setup(r => r.RegisterCustomerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync((CustomerEntity?)null);

            var result = await _service.RegisterAsync(request);

            Assert.Null(result);
        }

        private static CustomerEntity BuildCustomer(string email) => new()
        {
            CustomerId   = Guid.NewGuid(),
            CustomerName = "Alice",
            Email        = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };
    }
}