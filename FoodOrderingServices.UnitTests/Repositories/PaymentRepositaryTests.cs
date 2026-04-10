using FoodOrderingServices.Infrastructure.Repositories;
using Xunit;

namespace FoodOrderingServices.UnitTests.Repositories
{
    public class PaymentRepositaryTests
    {
        private readonly PaymentRepositary _repository = new();

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsTrue_Always()
        {
            var result = await _repository.ProcessPaymentAsync();

            Assert.True(result);
        }
    }
}