using System.ComponentModel.DataAnnotations;

namespace Customer.Core.Entity
{
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
