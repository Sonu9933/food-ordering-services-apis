using Customer.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a customer's order placed at a restaurant, including order details, status, and associated customer
    /// and restaurant information.
    /// </summary>
    /// <remarks>An order contains references to the customer who placed it, the restaurant fulfilling it, and
    /// a collection of order details representing the individual items ordered. The order status indicates the current
    /// processing state, and the total amount reflects the sum of all items in the order. This class is typically used
    /// to track and manage orders within a food ordering system.</remarks>
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; }
        public Guid CustomerID { get; set; }
        public Guid RestaurantID { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
