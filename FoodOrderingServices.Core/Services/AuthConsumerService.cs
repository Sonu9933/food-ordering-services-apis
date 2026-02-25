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

        public async Task<AuthenticationResponse?> AuthenticateAsync(LoginCustomerRequest loginRequest)
        {
            var customer = await _customerAuthRepositary.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);
            if (customer == null)
            {
                return null;
            }

            return new AuthenticationResponse()
            {
                Email = customer.Email,
                Id = customer.CustomerId,
                FullName = customer.CustomerName,
                Token = _jwtTokenService.GenerateToken(customer),
                ExpiresAt = DateTime.UtcNow,
            };
        }

        public async Task<FoodOrderingServices.Core.Entity.Customer?> RegisterAsync(RegisterCustomerRequest registerRequest)
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
