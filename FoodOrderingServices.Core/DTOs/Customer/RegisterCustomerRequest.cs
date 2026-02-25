namespace FoodOrderingServices.Core.DTOs.Customer
{
    public class RegisterCustomerRequest
    {
        public string ConsumerName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
