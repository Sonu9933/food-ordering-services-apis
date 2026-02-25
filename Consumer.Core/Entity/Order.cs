using Customer.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
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
