namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    public class RestaurantDetailDTO
    {
        public Guid RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
    }
}
