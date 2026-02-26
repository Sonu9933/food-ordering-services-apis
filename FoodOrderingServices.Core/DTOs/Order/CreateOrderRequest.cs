namespace FoodOrderingServices.Core.DTOs.Order
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid RestaurantID { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = [];

    }
}
