using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a customer account, including personal information, authentication details, and associated orders
    /// within the system.
    /// </summary>
    /// <remarks>The Customer class provides properties for identifying and authenticating a customer, as well
    /// as tracking account creation, updates, and login activity. It also maintains a collection of orders placed by
    /// the customer. This class is typically used as part of the application's data model for managing customer-related
    /// operations.</remarks>
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
