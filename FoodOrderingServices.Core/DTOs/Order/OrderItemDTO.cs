namespace FoodOrderingServices.Core.DTOs.Order
{
    /// <summary>
    /// Represents a data transfer object for an individual item within an order, including details such as the
    /// associated restaurant, item identification, name, description, price, and quantity.
    /// </summary>
    /// <remarks>This class is typically used to transfer order item information between application layers,
    /// such as from the data access layer to the presentation layer. Each property provides essential information
    /// required for processing and displaying order items.</remarks>
    public class OrderItemDTO
    {
        public Guid RestaurantID { get; set; }
        public Guid ItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
