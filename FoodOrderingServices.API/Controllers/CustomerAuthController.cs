using Asp.Versioning;
using Customer.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FoodOrderingServices.API.Controllers
{
    /// <summary>
    /// Handles customer authentication operations including login and registration.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("limiting")]
    public class CustomerAuthController : ControllerBase
    {
        private readonly IAuthCustomerService authCustomerService;
        private readonly ILogger<CustomerAuthController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAuthController"/> class.
        /// </summary>
        /// <param name="authCustomerService">The customer authentication service.</param>
        /// <param name="logger">The logger for recording application events.</param>
        /// <exception cref="ArgumentNullException">Thrown when authCustomerService is null.</exception>
        public CustomerAuthController(IAuthCustomerService authCustomerService, ILogger<CustomerAuthController> logger)
        {
            this.authCustomerService = authCustomerService ?? throw new ArgumentNullException(nameof(authCustomerService));
            this.logger = logger;
        }

        /// <summary>
        /// Authenticates a customer with their login credentials.
        /// </summary>
        /// <param name="loginRequest">The customer login details.</param>
        /// <returns>Authentication response containing customer information or error message.</returns>
        /// <response code="200">Authentication successful.</response>
        /// <response code="401">Authentication failed - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during login.</response>
        [ApiVersion(1)]
        [HttpPost("login-consumer")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthenticationResponse>> LoginConsumerAsync(LoginCustomerRequest loginRequest)
        {
            try
            {
                var result = await authCustomerService.AuthenticateAsync(loginRequest);
                if (result is null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogWarning(exception, "Login failed for email: {Email}", loginRequest.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error during login for email: {Email}", loginRequest.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Registers a new customer account.
        /// </summary>
        /// <param name="register">The customer registration details.</param>
        /// <returns>Registration result message or error details.</returns>
        /// <response code="200">Registration successful.</response>
        /// <response code="400">Registration failed - invalid input or business rule violation.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [ApiVersion(1)]
        [HttpPost("register-consumer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ConsumerRegistrationAsync(RegisterCustomerRequest register)
        {
            try
            {
                var result = await authCustomerService.RegisterAsync(register);
                if (result is null)
                {
                    return BadRequest(new { message = $"Can't register the consumer {register.Email} with provided details" });
                }

                return Ok(new { message = $"Consumer email {result.Email} is registered now" });
            }
            catch (InvalidOperationException exception)
            {
                // Business rule violation (e.g., email already exists)
                logger.LogWarning(exception, "Registration failed for email: {Email}", register.Email);
                return BadRequest(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                // Unexpected error
                logger.LogError(exception, "Unexpected error during registration for email: {Email}", register.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }
    }
}
