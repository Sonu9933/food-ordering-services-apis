namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    /// <summary>
    /// Represents the details of a restaurant, including its unique identifier, name, location, and contact
    /// information.
    /// </summary>
    /// <remarks>This class is used to transfer restaurant details between different layers of the
    /// application. Ensure that the RestaurantID is unique for each restaurant instance.</remarks>
    public class RestaurantDetailDTO
    {
        public Guid RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
    }
}
