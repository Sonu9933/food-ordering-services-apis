using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.DTOs.Restaurant
{
    /// <summary>
    /// Represents a request to create a new restaurant, including its name, location, and contact number.
    /// </summary>
    /// <remarks>Use this class to encapsulate the information required when adding a new restaurant to the
    /// system. All properties should be populated with valid, non-empty values before submitting the request.</remarks>
    public class AddRestaurantRequest
    {
        [Required(ErrorMessage = "Restaurant name can't be empty/null")]
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Location can't be empty/null")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Contact number can't be empty/null")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(10, ErrorMessage = "Contact number can't exceed 10 characters")]
        public string ContactNumber { get; set; }
    }
}
