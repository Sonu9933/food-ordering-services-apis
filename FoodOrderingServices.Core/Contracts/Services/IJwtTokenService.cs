using System.Security.Claims;

namespace FoodOrderingServices.Core.Contracts.Services
{
    /// <summary>
    /// Provides methods for generating and validating JSON Web Tokens (JWT) for user authentication.
    /// </summary>
    /// <remarks>This interface is designed to facilitate secure token management in applications that require
    /// user authentication. Implementations should ensure that tokens are generated with appropriate claims and that
    /// validation checks for token integrity and expiration.</remarks>
    public interface IJwtTokenService
    {
        string GenerateToken(Entity.Customer customer);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
