using FoodOrderingServices.Core.DTOs.Customer;

namespace FoodOrderingServices.Core.Contracts.Services
{
    /// <summary>
    /// Defines methods for authenticating and registering customers asynchronously.
    /// </summary>
    /// <remarks>Implementations of this interface should ensure secure handling of authentication credentials
    /// and enforce appropriate validation rules during registration. All methods are asynchronous and may involve
    /// external data sources or services.</remarks>
    public interface IAuthCustomerService
    {
        Task<AuthenticationResponse?> AuthenticateAsync(LoginCustomerRequest loginRequest);
        Task<Entity.Customer?> RegisterAsync(RegisterCustomerRequest registerRequest);
    }
}
