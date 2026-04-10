using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Services;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepositary>   _mockRepo;
        private readonly PaymentService             _service;

        public PaymentServiceTests()
        {
            _mockRepo = new Mock<IPaymentRepositary>();
            _service  = new PaymentService(_mockRepo.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsTrue_WhenRepositorySucceeds()
        {
            _mockRepo.Setup(r => r.ProcessPaymentAsync()).ReturnsAsync(true);

            var result = await _service.ProcessPaymentAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFalse_WhenRepositoryFails()
        {
            _mockRepo.Setup(r => r.ProcessPaymentAsync()).ReturnsAsync(false);

            var result = await _service.ProcessPaymentAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessPaymentAsync_CallsRepositoryOnce()
        {
            _mockRepo.Setup(r => r.ProcessPaymentAsync()).ReturnsAsync(true);

            await _service.ProcessPaymentAsync();

            _mockRepo.Verify(r => r.ProcessPaymentAsync(), Times.Once);
        }
    }
}