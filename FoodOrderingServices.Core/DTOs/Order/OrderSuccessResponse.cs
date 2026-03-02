using Customer.Core.Enum;

namespace FoodOrderingServices.Core.DTOs.Order
{
    /// <summary>
    /// Represents the response returned when an order is placed successfully, containing key details about the order.
    /// </summary>
    /// <remarks>This class is typically used to convey the outcome of an order placement operation to the
    /// client. It includes the unique identifier of the order, its current status, and the estimated time of arrival.
    /// The information provided by this response enables clients to track and manage their orders
    /// effectively.</remarks>
    public class OrderSuccessResponse
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string ETA { get; set; }
    }
}
