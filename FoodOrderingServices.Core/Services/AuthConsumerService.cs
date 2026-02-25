using Customer.Core.Contracts.Services;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Customer;

namespace ConsumerEnpoints.Services
{
    public class AuthConsumerService : IAuthCustomerService
    {
        public ICustomerRepositary _customerAuthRepositary;
        public IJwtTokenService _jwtTokenService;

        public AuthConsumerService(ICustomerRepositary customerAuthRepositary, IJwtTokenService jwtTokenService)
        {
            _customerAuthRepositary = customerAuthRepositary;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse?> AuthenticateAsync(LoginRequest loginRequest)
        {
            var customer = await _customerAuthRepositary.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);
            if (customer == null)
            {
                return null;
            }

            return new AuthResponse()
            {
                Email = customer.Email,
                Id = customer.CustomerId,
                FullName = customer.CustomerName,
                Token = _jwtTokenService.GenerateToken(customer),
                ExpiresAt = DateTime.UtcNow,
            };
        }

        public async Task<FoodOrderingServices.Core.Entity.Customer?> RegisterAsync(RegisterRequest registerRequest)
        {
            var registerCustomer = await _customerAuthRepositary
                .RegisterCustomerAsync(registerRequest.ConsumerName, 
                registerRequest.Email, 
                registerRequest.Password);

            if (registerCustomer == null) {
                return null;
            }

            return registerCustomer;
        }
    }
}
