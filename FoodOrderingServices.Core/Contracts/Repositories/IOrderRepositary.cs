using FoodOrderingServices.Core.DTOs.Order;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Repositories
{
    /// <summary>
    /// Represents a contract for asynchronously creating and retrieving orders from a data store.  
    /// </summary>
    /// <remarks>Implementations of this interface should ensure that all operations are performed
    /// asynchronously to support scalable and responsive applications. Methods may return <see langword="null"/> if the
    /// requested order or orders are not found. It is recommended that implementations handle data consistency and
    /// error scenarios according to the application's requirements.</remarks>
    public interface IOrderRepositary
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>?> GetOrdersByCustomerIdAsync(Guid customerId);
    }
}
