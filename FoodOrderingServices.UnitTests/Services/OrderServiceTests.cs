using Customer.Core.Enum;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Core.Services;
using Moq;
using Xunit;

namespace FoodOrderingServices.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepositary> _mockRepo;
        private readonly OrderService           _service;

        public OrderServiceTests()
        {
            _mockRepo = new Mock<IOrderRepositary>();
            _service  = new OrderService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsSuccessResponse_WhenOrderCreated()
        {
            var orderId = Guid.NewGuid();
            var request = BuildOrderRequest();
            var order   = new Order { OrderID = orderId, Status = OrderStatus.Confirmed };

            _mockRepo.Setup(r => r.CreateOrderAsync(request)).ReturnsAsync(order);

            var result = await _service.CreateOrderAsync(request);

            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(OrderStatus.Confirmed, result.OrderStatus);
            Assert.Equal("30 mins", result.ETA);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsEmptyOrderId_WhenRepositoryReturnsNull()
        {
            var request = BuildOrderRequest();
            _mockRepo.Setup(r => r.CreateOrderAsync(request)).ReturnsAsync((Order)null!);

            var result = await _service.CreateOrderAsync(request);

            Assert.Equal(Guid.Empty, result.OrderId);
            Assert.Equal(OrderStatus.None, result.OrderStatus);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenFound()
        {
            var orderId = Guid.NewGuid();
            var order   = new Order { OrderID = orderId };

            _mockRepo.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            var result = await _service.GetOrderByIdAsync(orderId);

            Assert.NotNull(result);
            Assert.Equal(orderId, result!.OrderID);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetOrderByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

            var result = await _service.GetOrderByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ReturnsOrders_WhenCustomerHasOrders()
        {
            var customerId = Guid.NewGuid();
            var orders     = new List<Order> { new Order { CustomerID = customerId } };

            _mockRepo.Setup(r => r.GetOrdersByCustomerIdAsync(customerId)).ReturnsAsync(orders);

            var result = await _service.GetOrdersByCustomerIdAsync(customerId);

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ReturnsNull_WhenNoOrders()
        {
            _mockRepo.Setup(r => r.GetOrdersByCustomerIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync(Enumerable.Empty<Order>());

            var result = await _service.GetOrdersByCustomerIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ReturnsNull_WhenRepositoryReturnsNull()
        {
            _mockRepo.Setup(r => r.GetOrdersByCustomerIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((IEnumerable<Order>?)null);

            var result = await _service.GetOrdersByCustomerIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        private static CreateOrderRequest BuildOrderRequest() => new()
        {
            CustomerId   = Guid.NewGuid(),
            RestaurantID = Guid.NewGuid(),
            OrderItems   = [new OrderItemDTO { ItemID = Guid.NewGuid(), ItemName = "Pizza", Price = 12, Quantity = 1, RestaurantID = Guid.NewGuid() }]
        };
    }
}