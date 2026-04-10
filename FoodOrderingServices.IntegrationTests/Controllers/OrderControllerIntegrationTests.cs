using FoodOrderingServices.Core.DTOs.Order;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class OrderControllerIntegrationTests : IntegrationTestBase
    {
        private const string PlaceOrderRoute = "api/Order/place-order";
        private const string GetByIdRoute    = "api/Order/get-order-detail-orderId";
        private const string GetByCustRoute  = "api/Order/get-order-detail-customerId";

        public OrderControllerIntegrationTests(IntegrationTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task PlaceOrder_ReturnsOk_WithValidRequest()
        {
            var request  = BuildOrderRequest();
            var response = await Client.PostAsJsonAsync(PlaceOrderRoute, request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PlaceOrder_ResponseContainsOrderId()
        {
            var request  = BuildOrderRequest();
            var response = await Client.PostAsJsonAsync(PlaceOrderRoute, request);

            var body = await response.Content.ReadFromJsonAsync<OrderSuccessResponse>();

            Assert.NotNull(body);
            Assert.NotEqual(Guid.Empty, body!.OrderId);
        }

        [Fact]
        public async Task PlaceOrder_ReturnsBadRequest_WhenBodyIsEmpty()
        {
            // Missing required fields → model validation fails
            var response = await Client.PostAsJsonAsync(PlaceOrderRoute, new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_WhenOrderExists()
        {
            var created = await PlaceOrderAndGetIdAsync();

            var response = await Client.GetAsync($"{GetByIdRoute}?orderId={created}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var response = await Client.GetAsync($"{GetByIdRoute}?orderId={Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task GetOrdersByCustomerId_ReturnsOk_WhenOrdersExist()
        {
            var customerId = Guid.NewGuid();
            await Client.PostAsJsonAsync(PlaceOrderRoute, BuildOrderRequest(customerId));

            var response = await Client.GetAsync($"{GetByCustRoute}?customerId={customerId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetOrdersByCustomerId_ReturnsNotFound_WhenNoOrdersExist()
        {
            var response = await Client.GetAsync($"{GetByCustRoute}?customerId={Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<Guid> PlaceOrderAndGetIdAsync()
        {
            var response = await Client.PostAsJsonAsync(PlaceOrderRoute, BuildOrderRequest());
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<OrderSuccessResponse>();
            return body!.OrderId;
        }

        private static CreateOrderRequest BuildOrderRequest(Guid? customerId = null) => new()
        {
            CustomerId   = customerId ?? Guid.NewGuid(),
            RestaurantID = Guid.NewGuid(),
            OrderItems   =
            [
                new OrderItemDTO
                {
                    RestaurantID = Guid.NewGuid(),
                    ItemID       = Guid.NewGuid(),
                    ItemName     = "Burger",
                    Description  = "Grilled",
                    Price        = 10,
                    Quantity     = 2
                }
            ]
        };
    }
}