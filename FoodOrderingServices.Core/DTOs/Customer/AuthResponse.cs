namespace FoodOrderingServices.Core.DTOs.Customer
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }
}
