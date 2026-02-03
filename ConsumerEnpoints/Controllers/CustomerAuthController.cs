using Asp.Versioning;
using ConsumerEnpoints.Models;
using Customer.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("limiting")]
    public class CustomerAuthController : ControllerBase
    {
        private readonly IAuthCustomerService authCustomerService;
        private readonly ILogger<CustomerAuthController> logger;

        public CustomerAuthController(IAuthCustomerService authCustomerService, ILogger<CustomerAuthController> logger)
        {
            this.authCustomerService = authCustomerService ?? throw new ArgumentNullException(nameof(authCustomerService));
            this.logger = logger;
        }

        [ApiVersion(1)]
        [HttpPost("login-consumer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> LoginConsumerAsync(LoginRequest loginRequest)
        {
            try
            {
                var result = authCustomerService.AuthenticateAsync(loginRequest);
                if (result is null)
                {
                    return Unauthorized(new { message = "Invalid email or password" }); ;
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

        [ApiVersion(1)]
        [HttpPost("register-consumer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CosumerRegistrationAsync(RegisterRequest register)
        {
            try
            {
                var result = await authCustomerService.RegisterAsync(register);
                if (result is null)
                {
                    return BadRequest($"Can't register the consumer {register.Email} with provided details");
                }

                return Ok($"Consumer email {result.Email} is register now");
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
