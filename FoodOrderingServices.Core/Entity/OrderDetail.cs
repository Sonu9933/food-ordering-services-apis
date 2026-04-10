using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a single line item within an <see cref="Order"/>.
    /// </summary>
    /// <remarks>
    /// Each <see cref="OrderDetail"/> captures a snapshot of a menu item at the time of ordering —
    /// specifically the <see cref="UnitPrice"/> at the moment of purchase — so that historical
    /// order records remain accurate even if menu prices change later.
    /// Cascade-deleted when the parent <see cref="Order"/> is removed.
    /// </remarks>
    public class OrderDetail
    {
        /// <summary>Unique identifier for the order detail line (primary key).</summary>
        [Key]
        public Guid OrderDetailID { get; set; }

        /// <summary>Foreign key linking back to the parent <see cref="Order"/>.</summary>
        public Guid OrderID { get; set; }

        /// <summary>Foreign key referencing the <see cref="MenuItems"/> item that was ordered.</summary>
        public Guid ItemID { get; set; }

        /// <summary>Number of units of the menu item included in this line.</summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Price per unit at the time the order was placed.
        /// Stored as a snapshot so price changes do not affect historical orders.
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>Navigation property — the parent order this line belongs to.</summary>
        public Order Order { get; set; }

        /// <summary>Navigation property — the menu item that was ordered.</summary>
        public MenuItems MenuItem { get; set; }
    }
}
