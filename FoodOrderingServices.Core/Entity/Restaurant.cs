using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a restaurant entity, including its identifying information, location, contact details, and associated
    /// orders.
    /// </summary>
    /// <remarks>This class is used to model restaurant data within the application. It provides properties
    /// for storing the restaurant's name, location, contact number, and timestamps for creation and updates. The Orders
    /// navigation property enables access to the collection of orders associated with the restaurant.</remarks>
    public class Restaurant
    {
        [Key]
        public Guid RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
