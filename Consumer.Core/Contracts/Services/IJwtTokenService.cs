using System.Security.Claims;

namespace Customer.Core.Contracts.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(Entity.Customer customer);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
