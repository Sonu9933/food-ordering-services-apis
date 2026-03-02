using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a menu item offered by a restaurant, including its identifying information, description, price, and
    /// category.
    /// </summary>
    /// <remarks>Each instance of this class corresponds to a single menu item associated with a specific
    /// restaurant. The menu item is uniquely identified by the ItemID property and is linked to a restaurant through
    /// the RestaurantID property. This class is typically used to manage and display menu options within a food
    /// ordering or restaurant management system.</remarks>
    public class MenuItems
    {
        [Key]
        public Guid ItemID { get; set; }
        public Guid RestaurantID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }

        // Navigation properties
        public Restaurant Restaurant { get; set; }
    }
}
