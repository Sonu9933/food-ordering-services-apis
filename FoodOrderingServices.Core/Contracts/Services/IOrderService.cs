using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    public interface IOrderService
    {
        Task<OrderSuccessResponse> CreateOrderAsync(CreateOrderRequest order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>?> GetOrdersByCustomerIdAsync(Guid customerId);
    }
}
