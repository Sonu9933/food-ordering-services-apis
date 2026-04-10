using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a registered customer account in the food ordering system.
    /// </summary>
    /// <remarks>
    /// Stores identity, credentials, and account lifecycle timestamps.
    /// The <see cref="Orders"/> navigation property exposes every order placed by this customer.
    /// Passwords are never stored in plain text — only a BCrypt hash is persisted via <see cref="PasswordHash"/>.
    /// </remarks>
    public class Customer
    {
        /// <summary>Unique identifier for the customer (primary key).</summary>
        [Key]
        public Guid CustomerId { get; set; }

        /// <summary>Display name chosen or provided during registration.</summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// BCrypt hash of the customer's password.
        /// Never expose this value in API responses.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Unique email address used for login and communication.
        /// Enforced as a unique index at the database level.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>UTC timestamp when the account was first created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp of the most recent account update.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UTC timestamp of the most recent successful login.
        /// <see langword="null"/> if the customer has never logged in after registration.
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>All orders placed by this customer (EF Core navigation property).</summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
