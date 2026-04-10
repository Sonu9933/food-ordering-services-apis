using Customer.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a customer's order placed at a specific restaurant.
    /// </summary>
    /// <remarks>
    /// An <see cref="Order"/> acts as the root aggregate for an order transaction.
    /// It references the placing <see cref="Customer"/> and the fulfilling <see cref="Restaurant"/>,
    /// and owns a collection of <see cref="OrderDetail"/> line items.
    /// <see cref="TotalAmount"/> is calculated at creation time as the sum of
    /// (quantity × unit price) across all order items.
    /// </remarks>
    public class Order
    {
        /// <summary>Unique identifier for the order (primary key).</summary>
        [Key]
        public Guid OrderID { get; set; }

        /// <summary>Foreign key linking to the <see cref="Customer"/> who placed this order.</summary>
        public Guid CustomerID { get; set; }

        /// <summary>Foreign key linking to the <see cref="Restaurant"/> fulfilling this order.</summary>
        public Guid RestaurantID { get; set; }

        /// <summary>
        /// Current lifecycle status of the order (e.g., Pending, Confirmed, Delivered).
        /// Updated as the order progresses through the fulfilment pipeline.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>UTC timestamp when the order was submitted.</summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Total monetary value of the order in the smallest currency unit.
        /// Computed as the sum of (Quantity × UnitPrice) for every <see cref="OrderDetail"/>.
        /// </summary>
        public int TotalAmount { get; set; }

        /// <summary>Navigation property — the customer who placed this order.</summary>
        public Customer Customer { get; set; }

        /// <summary>Navigation property — the restaurant fulfilling this order.</summary>
        public Restaurant Restaurant { get; set; }

        /// <summary>
        /// Individual line items that make up the order.
        /// Cascade-deleted when the parent <see cref="Order"/> is removed.
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
