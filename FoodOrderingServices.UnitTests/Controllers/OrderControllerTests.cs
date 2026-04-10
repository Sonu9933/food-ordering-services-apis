using Customer.Core.Enum;
using FoodOrderingServices.API.Controllers;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService>                    _mockOrderService;
        private readonly Mock<ILogger<RestaurantController>>    _mockLogger;
        private readonly OrderController                        _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger       = new Mock<ILogger<RestaurantController>>();
            _controller       = new OrderController(_mockOrderService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsOk_WhenOrderCreatedSuccessfully()
        {
            var orderId  = Guid.NewGuid();
            var request  = BuildOrderRequest();
            var response = new OrderSuccessResponse
            {
                OrderId     = orderId,
                OrderStatus = OrderStatus.Confirmed,
                ETA         = "30 mins"
            };

            _mockOrderService.Setup(s => s.CreateOrderAsync(request)).ReturnsAsync(response);

            var result = await _controller.CreateOrderAsync(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var body     = Assert.IsType<OrderSuccessResponse>(okResult.Value);
            Assert.Equal(orderId, body.OrderId);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsBadRequest_WhenOrderIdIsEmpty()
        {
            var request  = BuildOrderRequest();
            var response = new OrderSuccessResponse { OrderId = Guid.Empty };

            _mockOrderService.Setup(s => s.CreateOrderAsync(request)).ReturnsAsync(response);

            var result = await _controller.CreateOrderAsync(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsUnauthorized_OnUnauthorizedAccessException()
        {
            var request = BuildOrderRequest();
            _mockOrderService.Setup(s => s.CreateOrderAsync(request))
                             .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateOrderAsync(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrderAsync_Returns500_OnUnexpectedException()
        {
            var request = BuildOrderRequest();
            _mockOrderService.Setup(s => s.CreateOrderAsync(request))
                             .ThrowsAsync(new Exception("Unexpected"));

            var result         = await _controller.CreateOrderAsync(request);
            var statusResult   = Assert.IsType<ObjectResult>(result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOk_WhenOrderFound()
        {
            var orderId = Guid.NewGuid();
            var order   = new Order { OrderID = orderId };

            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            var result = await _controller.GetOrderByIdAsync(orderId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNotFound_WhenOrderNotFound()
        {
            var orderId = Guid.NewGuid();
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null);

            var result = await _controller.GetOrderByIdAsync(orderId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Returns500_OnUnexpectedException()
        {
            var orderId = Guid.NewGuid();
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId))
                             .ThrowsAsync(new Exception("DB error"));

            var result       = await _controller.GetOrderByIdAsync(orderId);
            var statusResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetOrderByCustomerIdAsync_ReturnsOk_WhenOrdersFound()
        {
            var customerId = Guid.NewGuid();
            var orders     = new List<Order> { new Order { CustomerID = customerId } };

            _mockOrderService.Setup(s => s.GetOrdersByCustomerIdAsync(customerId))
                             .ReturnsAsync(orders);

            var result = await _controller.GetOrderByCustomerIdAsync(customerId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderByCustomerIdAsync_ReturnsNotFound_WhenNoOrders()
        {
            var customerId = Guid.NewGuid();
            _mockOrderService.Setup(s => s.GetOrdersByCustomerIdAsync(customerId))
                             .ReturnsAsync((IEnumerable<Order>?)null);

            var result = await _controller.GetOrderByCustomerIdAsync(customerId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        private static CreateOrderRequest BuildOrderRequest() => new()
        {
            CustomerId   = Guid.NewGuid(),
            RestaurantID = Guid.NewGuid(),
            OrderItems   =
            [
                new OrderItemDTO
                {
                    RestaurantID = Guid.NewGuid(),
                    ItemID       = Guid.NewGuid(),
                    ItemName     = "Burger",
                    Price        = 10,
                    Quantity     = 2
                }
            ]
        };
    }
}