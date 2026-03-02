namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    /// <summary>
    /// Represents a request to create a new restaurant, including its name, location, and contact number.
    /// </summary>
    /// <remarks>Use this class to encapsulate the information required when adding a new restaurant to the
    /// system. All properties should be populated with valid, non-empty values before submitting the request.</remarks>
    public class AddRestaurantRequest
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
    }
}
