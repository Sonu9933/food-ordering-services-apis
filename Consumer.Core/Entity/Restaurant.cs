using System.ComponentModel.DataAnnotations;

namespace Customer.Core.Entity
{
    public class Restaurant
    {
        [Key]
        public Guid RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
