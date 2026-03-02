using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a single item entry within an order, including the associated menu item, quantity, and unit price.
    /// </summary>
    /// <remarks>The OrderDetail class links an order to its individual menu items, enabling detailed tracking
    /// of each component in an order. Each instance uniquely identifies a menu item ordered as part of a specific
    /// order, along with the quantity and price at the time of ordering. This class is typically used in conjunction
    /// with Order and MenuItems to manage and retrieve comprehensive order information.</remarks>
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailID { get; set; }
        public Guid OrderID { get; set; }
        public Guid ItemID { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public Order Order { get; set; }

        // Navigation properties
        public MenuItems MenuItem { get; set; }

    }
}
