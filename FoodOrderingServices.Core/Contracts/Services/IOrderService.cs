using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    /// <summary>
    /// Defines a contract for managing orders, including creating new orders and retrieving existing orders by their
    /// identifiers or associated customer.
    /// </summary>
    /// <remarks>Implementations of this interface should provide asynchronous, thread-safe operations for
    /// order management. Methods may throw exceptions for invalid input or if an order cannot be found. Callers should
    /// handle potential exceptions and ensure that provided identifiers are valid. This interface is intended to
    /// support scalable and responsive order processing in distributed or multi-threaded environments.</remarks>
    public interface IOrderService
    {
        Task<OrderSuccessResponse> CreateOrderAsync(CreateOrderRequest order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>?> GetOrdersByCustomerIdAsync(Guid customerId);
    }
}
