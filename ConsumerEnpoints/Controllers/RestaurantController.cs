using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Restaurant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FoodOrderingServices.API.Controllers
{
    /// <summary>
    /// Handles restaurant registration and update operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("limiting")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService restaurantService;
        private readonly ILogger<RestaurantController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestaurantController"/> class.
        /// </summary>
        /// <param name="restaurantService">The restaurant service for handling business logic.</param>
        /// <param name="logger">The logger for recording application events.</param>
        public RestaurantController(
            IRestaurantService restaurantService, 
            ILogger<RestaurantController> logger)
        {
            this.restaurantService = restaurantService;
            this.logger = logger;
        }

        /// <summary>
        /// Registers a new restaurant.
        /// </summary>
        /// <param name="register">The restaurant registration details.</param>
        /// <returns>A message indicating successful registration or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpPost("register-restaurant")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> RestaurantRegistrationAsync(RegisterRequest register)
        {
            try
            {
                var result = await restaurantService.AddRestaurentAsync(register);

                if(result == null)
                {
                    logger.LogWarning($"Failed to register the restaurant: {register.RestaurantName}");
                    return BadRequest(new { message = "Failed to register the restaurant" });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogWarning(exception, "Unauthorized access during restaurant registration");
                return Unauthorized(new { message = "Invalid credentials provided for registration" });
            }
            catch (Exception exception) 
            {
                logger.LogError(exception, $"Unexpected error while registering the restaurant: {register.RestaurantName}");
                return StatusCode(500, new { message = "An error occurred during registration of the restaurant" });
            }
        }

        /// <summary>
        /// Updates existing restaurant details.
        /// </summary>
        /// <param name="register">The updated restaurant details.</param>
        /// <returns>A message indicating successful update or an error message.</returns>
        /// <response code="200">Restaurant updated successfully.</response>
        /// <response code="400">Restaurant update failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during update.</response>
        [HttpPost("update-restaurant")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdateRestaurantDetailsAsync(RegisterRequest register)
        {
            try
            {
                var result = await restaurantService.UpdateRestaurentAsync(register);

                if (result == null)
                {
                    logger.LogWarning($"Failed to update the restaurant: {register.RestaurantName}");
                    return BadRequest(new { message = "Failed to update the restaurant" });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogWarning(exception, "Unauthorized access during restaurant update");
                return Unauthorized(new { message = "Invalid credentials provided for update" });
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Unexpected error while updating the restaurant: {register.RestaurantName}");
                return StatusCode(500, new { message = "An error occurred during update of the restaurant" });
            }
        }
    }
}
