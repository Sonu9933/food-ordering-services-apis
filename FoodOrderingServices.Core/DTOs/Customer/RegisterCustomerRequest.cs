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
        public string ConsumerName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
