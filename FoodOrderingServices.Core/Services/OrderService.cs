using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Services
{
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
