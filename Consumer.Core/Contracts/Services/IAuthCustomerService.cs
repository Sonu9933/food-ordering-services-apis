using ConsumerEnpoints.Models;

namespace Customer.Core.Contracts.Services
{
    public interface IAuthCustomerService
    {
        Task<AuthResponse?> AuthenticateAsync(LoginRequest loginRequest);
        Task<Entity.Customer?> RegisterAsync(RegisterRequest registerRequest);
    }
}
