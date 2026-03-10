using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingServices.Core.DTOs.Customer
{
    /// <summary>
    /// Represents the data required to register a new customer account.
    /// </summary>
    /// <remarks>This class encapsulates the essential information needed for customer registration, including
    /// the consumer's name, password, and email address. Ensure that the password meets security requirements and that
    /// the email address is in a valid format before processing the registration.</remarks>
    public class RegisterCustomerRequest
    {
        [Required(ErrorMessage = "Please enter the customer name")]
        [MaxLength(6, ErrorMessage ="Maximum character is 6")]
        [MinLength(1, ErrorMessage = "Maximum character is 6")]
        public string ConsumerName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Please enter the password")]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the email address")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}
