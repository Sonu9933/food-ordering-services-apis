using ConsumerEnpoints.Models;
using Customer.Core.Contracts.Repositories;
using Customer.Core.Contracts.Services;

namespace ConsumerEnpoints.Services
{
    public class AuthConsumerService : IAuthCustomerService
    {
        public ICustomerRepositary customerAuthRepositary { get; }
        public IJwtTokenService jwtTokenService { get; }

        public AuthConsumerService(ICustomerRepositary customerAuthRepositary, IJwtTokenService jwtTokenService)
        {
            this.customerAuthRepositary = customerAuthRepositary;
            this.jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse?> AuthenticateAsync(LoginRequest loginRequest)
        {
            var customer = await customerAuthRepositary.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);
            if (customer == null)
            {
                return null;
            }

            return new AuthResponse()
            {
                Email = customer.Email,
                Id = customer.CustomerId,
                FullName = customer.CustomerName,
                Token = jwtTokenService.GenerateToken(customer),
                ExpiresAt = DateTime.UtcNow,
            };
        }

        public async Task<Customer.Core.Entity.Customer?> RegisterAsync(RegisterRequest registerRequest)
        {
            var registerCustomer = await customerAuthRepositary
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
