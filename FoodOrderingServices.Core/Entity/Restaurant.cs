using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
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
