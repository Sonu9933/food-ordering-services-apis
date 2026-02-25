using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.DTOs.Customer
{
    public class LoginCustomerRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
