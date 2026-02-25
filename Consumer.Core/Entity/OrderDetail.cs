using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
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
