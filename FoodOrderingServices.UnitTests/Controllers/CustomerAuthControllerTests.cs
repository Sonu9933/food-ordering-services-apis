using FoodOrderingServices.API.Controllers;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Controllers
{
    public class CustomerAuthControllerTests
    {
        private readonly Mock<IAuthCustomerService>             _mockAuthService;
        private readonly Mock<ILogger<CustomerAuthController>>  _mockLogger;
        private readonly CustomerAuthController                 _controller;

        public CustomerAuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthCustomerService>();
            _mockLogger      = new Mock<ILogger<CustomerAuthController>>();
            _controller      = new CustomerAuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginConsumerAsync_ReturnsOk_WithValidCredentials()
        {
            var request  = new LoginCustomerRequest { Email = "user@test.com", Password = "pass" };
            var authResp = new AuthenticationResponse { Email = "user@test.com", Token = "jwt-token" };

            _mockAuthService.Setup(s => s.AuthenticateAsync(request)).ReturnsAsync(authResp);

            var result   = await _controller.LoginConsumerAsync(request);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.IsType<AuthenticationResponse>(okResult.Value);
        }

        [Fact]
        public async Task LoginConsumerAsync_ReturnsUnauthorized_WhenServiceReturnsNull()
        {
            var request = new LoginCustomerRequest { Email = "user@test.com", Password = "wrong" };
            _mockAuthService.Setup(s => s.AuthenticateAsync(request)).ReturnsAsync((AuthenticationResponse?)null);

            var result = await _controller.LoginConsumerAsync(request);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task LoginConsumerAsync_ReturnsUnauthorized_OnUnauthorizedAccessException()
        {
            var request = new LoginCustomerRequest { Email = "user@test.com", Password = "pass" };
            _mockAuthService.Setup(s => s.AuthenticateAsync(request))
                            .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.LoginConsumerAsync(request);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task LoginConsumerAsync_Returns500_OnUnexpectedException()
        {
            var request = new LoginCustomerRequest { Email = "user@test.com", Password = "pass" };
            _mockAuthService.Setup(s => s.AuthenticateAsync(request))
                            .ThrowsAsync(new Exception("Unexpected error"));

            var result       = await _controller.LoginConsumerAsync(request);
            var statusResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task ConsumerRegistrationAsync_ReturnsOk_WhenRegistered()
        {
            var request  = BuildRegisterRequest();
            var customer = new FoodOrderingServices.Core.Entity.Customer
            {
                CustomerId   = Guid.NewGuid(),
                Email        = request.Email,
                CustomerName = request.ConsumerName
            };

            _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(customer);

            var result = await _controller.ConsumerRegistrationAsync(request);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task ConsumerRegistrationAsync_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            var request = BuildRegisterRequest();
            _mockAuthService.Setup(s => s.RegisterAsync(request))
                            .ReturnsAsync((FoodOrderingServices.Core.Entity.Customer?)null);

            var result = await _controller.ConsumerRegistrationAsync(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ConsumerRegistrationAsync_ReturnsBadRequest_OnInvalidOperationException()
        {
            var request = BuildRegisterRequest();
            _mockAuthService.Setup(s => s.RegisterAsync(request))
                            .ThrowsAsync(new InvalidOperationException("Email already exists"));

            var result = await _controller.ConsumerRegistrationAsync(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ConsumerRegistrationAsync_Returns500_OnUnexpectedException()
        {
            var request = BuildRegisterRequest();
            _mockAuthService.Setup(s => s.RegisterAsync(request))
                            .ThrowsAsync(new Exception("Unexpected"));

            var result       = await _controller.ConsumerRegistrationAsync(request);
            var statusResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        private static RegisterCustomerRequest BuildRegisterRequest() => new()
        {
            ConsumerName = "Alice",
            Email        = "alice@test.com",
            Password     = "SecurePass123!"
        };
    }
}