using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class PaymentControllerIntegrationTests : IntegrationTestBase
    {
        private const string ProcessRoute = "api/Payment/process";

        public PaymentControllerIntegrationTests(IntegrationTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task ProcessPayment_ReturnsOk()
        {
            var response = await Client.PostAsJsonAsync(ProcessRoute, new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ProcessPayment_ResponseContainsSuccessTrue()
        {
            var response = await Client.PostAsJsonAsync(ProcessRoute, new { });
            var body     = await response.Content.ReadFromJsonAsync<PaymentResponse>();

            Assert.NotNull(body);
            Assert.True(body!.Success);
        }

        // Local DTO to deserialize the anonymous response
        private sealed record PaymentResponse(bool Success);
    }
}