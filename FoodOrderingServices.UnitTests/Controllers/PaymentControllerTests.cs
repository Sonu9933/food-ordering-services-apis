using FoodOrderingServices.API.Controllers;
using FoodOrderingServices.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _mockService;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _mockService = new Mock<IPaymentService>();
            _controller  = new PaymentController(_mockService.Object);
        }

        [Fact]
        public async Task ProcessPayment_ReturnsOk_WhenServiceReturnsTrue()
        {
            _mockService.Setup(s => s.ProcessPaymentAsync()).ReturnsAsync(true);

            var result = await _controller.ProcessPayment();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ProcessPayment_ResponseContainsSuccessTrue_WhenServiceReturnsTrue()
        {
            _mockService.Setup(s => s.ProcessPaymentAsync()).ReturnsAsync(true);

            var result    = await _controller.ProcessPayment();
            var okResult  = Assert.IsType<OkObjectResult>(result);
            var success   = okResult.Value!.GetType().GetProperty("Success")!.GetValue(okResult.Value);

            Assert.True((bool)success!);
        }

        [Fact]
        public async Task ProcessPayment_ResponseContainsSuccessFalse_WhenServiceReturnsFalse()
        {
            _mockService.Setup(s => s.ProcessPaymentAsync()).ReturnsAsync(false);

            var result   = await _controller.ProcessPayment();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var success  = okResult.Value!.GetType().GetProperty("Success")!.GetValue(okResult.Value);

            Assert.False((bool)success!);
        }

        [Fact]
        public async Task ProcessPayment_CallsServiceOnce()
        {
            _mockService.Setup(s => s.ProcessPaymentAsync()).ReturnsAsync(true);

            await _controller.ProcessPayment();

            _mockService.Verify(s => s.ProcessPaymentAsync(), Times.Once);
        }
    }
}