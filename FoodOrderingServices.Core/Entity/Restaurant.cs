using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.Entity
{
    /// <summary>
    /// Represents a restaurant registered on the platform.
    /// </summary>
    /// <remarks>
    /// A restaurant is the central entity around which menus and orders are organised.
    /// Both <see cref="RestaurantName"/> and <see cref="ContactNumber"/> are enforced as
    /// unique at the repository level to prevent duplicate registrations.
    /// </remarks>
    public class Restaurant
    {
        /// <summary>Unique identifier for the restaurant (primary key).</summary>
        [Key]
        public Guid RestaurantID { get; set; }

        /// <summary>
        /// Public-facing name of the restaurant.
        /// Must be unique across all registered restaurants.
        /// </summary>
        public string RestaurantName { get; set; }

        /// <summary>Physical or descriptive address/location of the restaurant.</summary>
        public string Location { get; set; }

        /// <summary>
        /// Contact phone number for the restaurant.
        /// Must be unique and is validated as a phone number format.
        /// Maximum length: 10 characters.
        /// </summary>
        public string ContactNumber { get; set; }

        /// <summary>UTC timestamp when the restaurant record was first created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>UTC timestamp of the most recent update to the restaurant record.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>All orders associated with this restaurant (EF Core navigation property).</summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
