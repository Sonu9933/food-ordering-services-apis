using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Services
{
    /// <summary>
    /// Provides operations for creating and retrieving orders within the system.
    /// </summary>
    /// <remarks>The OrderService interacts with an order repository to manage order data asynchronously. It
    /// is designed to be used in scenarios where order creation and retrieval are required, such as processing customer
    /// orders or displaying order history. Ensure that input data is validated before invoking service methods. This
    /// class is not thread-safe; if used concurrently, external synchronization is required.</remarks>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepositary _orderRepositary;

        public OrderService(IOrderRepositary orderRepositary)
        {
            _orderRepositary = orderRepositary;
        }

        public async Task<OrderSuccessResponse> CreateOrderAsync(CreateOrderRequest order)
        {
            var result = await _orderRepositary.CreateOrderAsync(order);

            if (result != null)
            {
                var orderSuccessResponse = new OrderSuccessResponse()
                {
                    ETA = "30 mins",
                    OrderId = result.OrderID,
                    OrderStatus = result.Status
                };

                return orderSuccessResponse;
            }

            return new OrderSuccessResponse()
            {
                ETA = "N/A",
                OrderId = Guid.Empty,
                OrderStatus = Customer.Core.Enum.OrderStatus.None
            };
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepositary.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }
            else
            {
                return order;
            }
        }

        public async Task<IEnumerable<Order>?> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            var orders = await _orderRepositary.GetOrdersByCustomerIdAsync(customerId);
            if (orders == null || !orders.Any())
                return null;
            else
                return orders;
        }
    }
}
