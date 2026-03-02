namespace FoodOrderingServices.Core.DTOs.Order
{
    /// <summary>
    /// Represents a request to create a new order, including the customer and restaurant details along with the items
    /// to be ordered.
    /// </summary>
    /// <remarks>The OrderItems property must contain at least one item to successfully create an order.
    /// Ensure that the CustomerId and RestaurantID are valid GUIDs representing existing entities.</remarks>
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid RestaurantID { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = [];

    }
}
