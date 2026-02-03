
using System.ComponentModel.DataAnnotations;

namespace Customer.Core.Entity
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
