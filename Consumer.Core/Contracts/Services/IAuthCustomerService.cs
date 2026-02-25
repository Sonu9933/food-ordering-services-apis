using FoodOrderingServices.Core.DTOs.Customer;

namespace Customer.Core.Contracts.Services
{
    public interface IAuthCustomerService
    {
        Task<AuthResponse?> AuthenticateAsync(LoginRequest loginRequest);
        Task<FoodOrderingServices.Core.Entity.Customer?> RegisterAsync(RegisterRequest registerRequest);
    }
}
