using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    /// <summary>
    /// Represents the details of a restaurant, including its unique identifier, name, location, and contact
    /// information.
    /// </summary>
    /// <remarks>This class is used to transfer restaurant details between different layers of the
    /// application. Ensure that the RestaurantID is unique for each restaurant instance.</remarks>
    public class RestaurantDetailDTO
    {
        [Required(ErrorMessage = "Restaurant id can't be empty/null")]
        public Guid RestaurantID { get; set; }

        [Required(ErrorMessage = "Restaurant name can't be empty/null")]
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Location can't be empty/null")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Contact number can't be empty/null")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ContactNumber { get; set; }
    }
}
