using Customer.API.Controllers;
using ConsumerEnpoints.Models;
using Customer.Core.Contracts.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Customer.UnitTests
{
    internal class CustomerAuthControllerTest
    {
        private CustomerAuthController _controller;
        private Mock<IAuthCustomerService> _authCustomerService;
        private Mock<ILogger<CustomerAuthController>> _logger;

        [SetUp]
        public void Setup()
        {
            _authCustomerService = new Mock<IAuthCustomerService>();
            _logger = new Mock<ILogger<CustomerAuthController>>();
            _controller = new CustomerAuthController(_authCustomerService.Object, _logger.Object);
        }

        [Test]
        public async Task LoginConsumerAsync_WhenUserLoginSuccessfull_ReturnsUser()
        {
            var loginRequest = new LoginRequest()
            { Email = "test@gmail.com",
             Password = "Test@!@#123"};

            var result = new AuthResponse()
            {
                Email = loginRequest.Email,
                ExpiresAt = DateTime.UtcNow,
                FullName = "Test",
                Id = Guid.NewGuid(),
                Token = "token"
            };

            _authCustomerService
                .Setup(s=>s.AuthenticateAsync(loginRequest))
                .ReturnsAsync(result);

            var customer = await _controller.LoginConsumerAsync(loginRequest);

            Assert.IsNotNull(customer);
        }

        [Test]
        public async Task CosumerRegistrationAsync_WhenUserRegisterSuccessfull_ReturnsMessage()
        {
            var registerRequest = new RegisterRequest()
            {
                ConsumerName = "Test",
                Email = "test@gmail.com",
                Password = "Test@!@#123"
            };

            var result = new Core.Entity.Customer()
            {
                Email = registerRequest.Email,
                CreatedAt = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test",  
                LastLogin = null,
                PasswordHash = "test",
                UpdatedAt = DateTime.UtcNow,
            };

            _authCustomerService
                .Setup(s => s.RegisterAsync(registerRequest))
                .ReturnsAsync(result);

            var customer = await _controller.CosumerRegistrationAsync(registerRequest);

            Assert.IsNotNull(customer);
        }
    }
}
