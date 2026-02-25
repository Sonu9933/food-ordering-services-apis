using System.Security.Claims;

namespace Customer.Core.Contracts.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(FoodOrderingServices.Core.Entity.Customer customer);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
