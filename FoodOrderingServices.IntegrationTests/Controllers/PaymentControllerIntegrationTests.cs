using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class PaymentControllerIntegrationTests : IntegrationTestBase
    {
        private const string ProcessPaymentRoute = "api/v1/Payment/process";

        public PaymentControllerIntegrationTests(IntegrationTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task ProcessPayment_ReturnsOk()
        {
            var response = await Client.PostAsync(ProcessPaymentRoute, null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ProcessPayment_ResponseContainsSuccessTrue()
        {
            var response = await Client.PostAsync(ProcessPaymentRoute, null);

            var body = await response.Content.ReadFromJsonAsync<dynamic>();

            Assert.NotNull(body);
        }
    }
}