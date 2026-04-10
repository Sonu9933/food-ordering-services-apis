namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    /// <summary>
    /// Represents a single item available on a restaurant's menu.
    /// </summary>
    public class MenuItemDTO
    {
        public Guid ItemID { get; set; }
        public Guid RestaurantID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
