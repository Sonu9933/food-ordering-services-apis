using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a single item on a restaurant's menu.
    /// </summary>
    /// <remarks>
    /// Menu items belong to exactly one <see cref="Restaurant"/> and are cascade-deleted
    /// when their parent restaurant is removed.
    /// They are referenced by <see cref="OrderDetail"/> records to link ordered items
    /// to their catalogue definitions.
    /// </remarks>
    public class MenuItems
    {
        /// <summary>Unique identifier for the menu item (primary key).</summary>
        [Key]
        public Guid ItemID { get; set; }

        /// <summary>Foreign key linking this item to its owning <see cref="Restaurant"/>.</summary>
        public Guid RestaurantID { get; set; }

        /// <summary>Customer-visible name of the dish or product (e.g., "Margherita Pizza").</summary>
        public string ItemName { get; set; }

        /// <summary>Optional description providing ingredient or preparation details.</summary>
        public string Description { get; set; }

        /// <summary>Unit price of the item in the smallest currency unit (e.g., pence/cents).</summary>
        public int Price { get; set; }

        /// <summary>Menu category the item belongs to (e.g., "Starters", "Mains", "Desserts").</summary>
        public string Category { get; set; }

        /// <summary>Navigation property — the restaurant that owns this menu item.</summary>
        public Restaurant Restaurant { get; set; }
    }
}
