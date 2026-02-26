namespace FoodOrderingServices.Core.DTOs.Order
{
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
