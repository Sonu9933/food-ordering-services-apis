namespace FoodOrderingServices.Core.DTOs.Customer
{
    /// <summary>
    /// Represents the response returned after a successful authentication, containing user identity and session
    /// information.
    /// </summary>
    /// <remarks>This class provides the authentication token, its expiration time, and essential user details
    /// such as the unique identifier, email address, and optional full name. The token can be used to authorize
    /// subsequent requests until the specified expiration time.</remarks>
    public class AuthenticationResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }
}
