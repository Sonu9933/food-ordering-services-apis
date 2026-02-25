using FoodOrderingServices.Core.DTOs.Customer;

namespace Customer.Core.Contracts.Services
{
    public interface IAuthCustomerService
    {
        Task<AuthenticationResponse?> AuthenticateAsync(LoginCustomerRequest loginRequest);
        Task<FoodOrderingServices.Core.Entity.Customer?> RegisterAsync(RegisterCustomerRequest registerRequest);
    }
}
