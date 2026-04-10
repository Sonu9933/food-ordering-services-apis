using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Customer;

namespace ConsumerEnpoints.Services
{
    /// <summary>
    /// Handles customer authentication and registration business logic.
    /// </summary>
    /// <remarks>
    /// Coordinates between <see cref="ICustomerRepositary"/> for credential verification/storage
    /// and <see cref="IJwtTokenService"/> for token issuance.
    /// This service intentionally contains no password-hashing logic — that concern
    /// belongs to the repository layer (BCrypt is applied in <see cref="ICustomerRepositary"/>).
    /// </remarks>
    public class AuthConsumerService : IAuthCustomerService
    {
        /// <summary>Repository used to look up and persist customer records.</summary>
        public ICustomerRepositary _customerAuthRepositary;

        /// <summary>Service used to generate and validate JWT bearer tokens.</summary>
        public IJwtTokenService _jwtTokenService;

        /// <summary>
        /// Initialises a new instance of <see cref="AuthConsumerService"/>.
        /// </summary>
        /// <param name="customerAuthRepositary">Data-access layer for customer credentials.</param>
        /// <param name="jwtTokenService">Token issuance and validation service.</param>
        public AuthConsumerService(ICustomerRepositary customerAuthRepositary, IJwtTokenService jwtTokenService)
        {
            _customerAuthRepositary = customerAuthRepositary;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Authenticates a customer using their email and password, returning a JWT on success.
        /// </summary>
        /// <param name="loginRequest">Email and password submitted by the caller.</param>
        /// <returns>
        /// An <see cref="AuthenticationResponse"/> containing the bearer token and customer identity,
        /// or <see langword="null"/> if no matching customer record is found.
        /// </returns>
        /// <remarks>
        /// Password verification is performed inside the repository (BCrypt hash comparison).
        /// If the repository returns <see langword="null"/> the credentials are invalid and
        /// no token is issued.
        /// </remarks>
        public async Task<AuthenticationResponse?> AuthenticateAsync(LoginCustomerRequest loginRequest)
        {
            var customer = await _customerAuthRepositary.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);
            if (customer == null)
            {
                return null;
            }

            return new AuthenticationResponse()
            {
                Email     = customer.Email,
                Id        = customer.CustomerId,
                FullName  = customer.CustomerName,
                Token     = _jwtTokenService.GenerateToken(customer),
                ExpiresAt = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Registers a new customer account with the provided details.
        /// </summary>
        /// <param name="registerRequest">Name, email, and password supplied by the caller.</param>
        /// <returns>
        /// The newly created <see cref="FoodOrderingServices.Core.Entity.Customer"/> entity,
        /// or <see langword="null"/> if the email address is already registered.
        /// </returns>
        /// <remarks>
        /// The raw password is forwarded to the repository, which is responsible for
        /// hashing it via BCrypt before persistence.
        /// </remarks>
        public async Task<FoodOrderingServices.Core.Entity.Customer?> RegisterAsync(RegisterCustomerRequest registerRequest)
        {
            var registerCustomer = await _customerAuthRepositary
                .RegisterCustomerAsync(registerRequest.ConsumerName,
                    registerRequest.Email,
                    registerRequest.Password);

            if (registerCustomer == null)
            {
                return null;
            }

            return registerCustomer;
        }
    }
}
