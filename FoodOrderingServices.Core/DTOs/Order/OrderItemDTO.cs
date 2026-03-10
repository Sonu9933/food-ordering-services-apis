using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Restaurant id can't be empty/null")]
        public Guid RestaurantID { get; set; }

        [Required(ErrorMessage = "Restaurant id can't be empty/null")]
        public Guid ItemID { get; set; }

        [Required(ErrorMessage = "Item name can't be empty/null")]
        public string ItemName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
    }
}
