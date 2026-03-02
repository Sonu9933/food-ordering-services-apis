using Customer.Core.Enum;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    /// <summary>
    /// Provides methods for creating and retrieving orders from the underlying database in an asynchronous manner.
    /// </summary>
    /// <remarks>This repository is intended for use in web applications to manage order data efficiently. It
    /// abstracts the data access logic for orders, supporting asynchronous operations to improve scalability and
    /// responsiveness. All methods interact with the application's database context and are designed to be used as part
    /// of a dependency injection setup.</remarks>
    public class OrderRepositary : IOrderRepositary
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderRepositary(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest order)
        {
            var orderEntity = new Order
            {
                OrderID = Guid.NewGuid(),
                CustomerID = order.CustomerId,
                RestaurantID = order.RestaurantID,
                Status = OrderStatus.Confirmed,
                OrderDate = DateTime.UtcNow,
                TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.Price)
            };

            var orderDetails = order.OrderItems.Select(item => new OrderDetail
            {
                OrderID = orderEntity.OrderID,
                OrderDetailID = Guid.NewGuid(),
                ItemID = item.ItemID,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            }).ToList();

            await _applicationDbContext.Orders.AddAsync(orderEntity);
            foreach (var detail in orderDetails)
            {
                await _applicationDbContext.OrderDetails.AddAsync(detail);
            }

            await _applicationDbContext.SaveChangesAsync();
            return orderEntity;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _applicationDbContext.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            return order;
        }
        public async Task<IEnumerable<Order>?> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            var orders = await _applicationDbContext.Orders
                .Where(o => o.CustomerID == customerId)
                .ToListAsync();

            return orders.Count > 0 ? orders : Enumerable.Empty<Order>();
        }
    }
}
