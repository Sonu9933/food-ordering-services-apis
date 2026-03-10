using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.DTOs.Customer
{
    /// <summary>
    /// Represents a request to authenticate a customer using their email address and password.
    /// </summary>
    /// <remarks>Both the Email and Password properties must be provided for a valid login request. The Email
    /// property must contain a valid email address format. This class is typically used as a data transfer object when
    /// submitting customer login information to an authentication endpoint.</remarks>
    public class LoginCustomerRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
