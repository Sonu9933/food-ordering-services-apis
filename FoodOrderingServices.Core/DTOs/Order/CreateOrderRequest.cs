using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Customer id can't be empty/null")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Restaurant id can't be empty/null")]
        public Guid RestaurantID { get; set; }

        //    MinLength(1) added: [Required] alone won't reject an empty list because
        //    the property initialiser (= []) means the value is never null.
        //    MinLength(1) fails model validation when the list is empty,
        //    which causes [ApiController] to return 400 before the action runs.
        [Required(ErrorMessage = "Order items can't be empty/null")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemDTO> OrderItems { get; set; } = [];
    }
}
