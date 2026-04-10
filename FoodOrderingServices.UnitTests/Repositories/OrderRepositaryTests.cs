using Customer.Core.Enum;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using FoodOrderingServices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodOrderingServices.UnitTests.Repositories
{
    public class OrderRepositaryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderRepositary      _repository;

        public OrderRepositaryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"OrderDb_{Guid.NewGuid():N}")
                .Options;

            _context    = new ApplicationDbContext(options);
            _repository = new OrderRepositary(_context);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsOrderWithId()
        {
            var request = BuildOrderRequest();

            var result = await _repository.CreateOrderAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.OrderID);
            Assert.Equal(request.CustomerId, result.CustomerID);
            Assert.Equal(OrderStatus.Confirmed, result.Status);
        }

        [Fact]
        public async Task CreateOrderAsync_PersistsOrderInDatabase()
        {
            var request = BuildOrderRequest();

            await _repository.CreateOrderAsync(request);

            Assert.Equal(1, await _context.Orders.CountAsync());
        }

        [Fact]
        public async Task CreateOrderAsync_PersistsOrderDetails()
        {
            var request = BuildOrderRequest(itemCount: 2);

            await _repository.CreateOrderAsync(request);

            Assert.Equal(2, await _context.OrderDetails.CountAsync());
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenExists()
        {
            var request = BuildOrderRequest();
            var created = await _repository.CreateOrderAsync(request);

            var result = await _repository.GetOrderByIdAsync(created.OrderID);

            Assert.NotNull(result);
            Assert.Equal(created.OrderID, result!.OrderID);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _repository.GetOrderByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_IncludesOrderDetails()
        {
            var request = BuildOrderRequest(itemCount: 2);
            var created = await _repository.CreateOrderAsync(request);

            var result = await _repository.GetOrderByIdAsync(created.OrderID);

            Assert.NotNull(result!.OrderDetails);
            Assert.Equal(2, result.OrderDetails.Count);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ReturnsOrders_WhenCustomerHasOrders()
        {
            var customerId = Guid.NewGuid();
            var request    = BuildOrderRequest(customerId: customerId);
            await _repository.CreateOrderAsync(request);

            var result = await _repository.GetOrdersByCustomerIdAsync(customerId);

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ReturnsEmpty_WhenNoOrders()
        {
            var result = await _repository.GetOrdersByCustomerIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Empty(result!);
        }

        private static CreateOrderRequest BuildOrderRequest(
            Guid? customerId = null,
            int   itemCount  = 1) => new()
        {
            CustomerId   = customerId ?? Guid.NewGuid(),
            RestaurantID = Guid.NewGuid(),
            OrderItems   = Enumerable.Range(0, itemCount).Select(_ => new OrderItemDTO
            {
                RestaurantID = Guid.NewGuid(),
                ItemID       = Guid.NewGuid(),
                ItemName     = "Burger",
                Price        = 10,
                Quantity     = 1
            }).ToList()
        };

        public void Dispose() => _context.Dispose();
    }
}