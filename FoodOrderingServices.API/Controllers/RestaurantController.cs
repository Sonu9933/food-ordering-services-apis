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
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestaurantController"/> class.
        /// </summary>
        /// <param name="restaurantService">The restaurant service for handling business logic.</param>
        /// <param name="logger">The logger for recording application events.</param>
        public RestaurantController(
            IRestaurantService restaurantService, 
            ILogger<RestaurantController> logger)
        {
            _restaurantService = restaurantService;
            _logger = logger;
        }


        /// <summary>
        /// Get a restaurent by id.
        /// </summary>
        /// /// <param name="restaurantId">The restaurant registration id.</param>
        /// <returns>A message with restaurant details or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpGet("get-restaurent-by-Id")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RestaurantDetailDTO>> GetRestaurentByIdAsync(Guid restaurantId) 
        {
            try
            {
                var result = await _restaurantService.GetAllRestaurentsAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access while fetching the restaurant");
                return Unauthorized(new { message = "Invalid credentials provided for fetching the restaurant" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while fetching the restaurant" + exception.Message);
                return StatusCode(500, new { message = "An error occurred while fetching the restaurant" });
            }
        }

        /// <summary>
        /// Get a restaurent by id.
        /// </summary>
        /// /// <param name="restaurantId">The restaurant registration id.</param>
        /// <returns>A message with deletion is succussfull or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpGet("delete-restaurent-by-Id")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> DeleteRestaurentByIdAsync(Guid restaurantId)
        {
            try
            {
                var result = await _restaurantService.DeleteRestaurentAsync(restaurantId);
                if(result == true)
                {
                    return Ok($"Deleted the restaurent : {restaurantId} successfully"); ;
                }

                return Ok($"Not abe to delete the restaurent : {restaurantId}");
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access while fetching the restaurant");
                return Unauthorized(new { message = "Invalid credentials provided for fetching the restaurant" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error whilet deleting the restaurent" + exception.Message);
                return StatusCode(500, new { message = "An error occurred while deleting the restaurent" });
            }
        }

        /// <summary>
        /// Get all the restaurent.
        /// </summary>
        /// <returns>A message with all the restaurants details or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpGet("get-all-restaurents")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RestaurantDetailDTO>> GetAllRestaurentAsync()
        {
            try
            {
                var result = await _restaurantService.GetAllRestaurentsAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access while fetching the restaurants");
                return Unauthorized(new { message = "Invalid credentials provided for fetching the restaurants" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while fetching the restaurants" + exception.Message);
                return StatusCode(500, new { message = "An error occurred while fetching the restaurants" });
            }
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
        public async Task<ActionResult<string>> RestaurantRegistrationAsync(AddRestaurantRequest register)
        {
            try
            {
                var result = await _restaurantService.AddRestaurentAsync(register);

                if(result == null)
                {
                    _logger.LogWarning($"Failed to register the restaurant: {register.RestaurantName}");
                    return BadRequest(new { message = "Failed to register the restaurant" });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access during restaurant registration");
                return Unauthorized(new { message = "Invalid credentials provided for registration" });
            }
            catch (Exception exception) 
            {
                _logger.LogError(exception, $"Unexpected error while registering the restaurant: {register.RestaurantName}");
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
        public async Task<ActionResult<string>> UpdateRestaurantDetailsAsync(AddRestaurantRequest register)
        {
            try
            {
                var result = await _restaurantService.UpdateRestaurentAsync(register);

                if (result == null)
                {
                    _logger.LogWarning($"Failed to update the restaurant: {register.RestaurantName}");
                    return BadRequest(new { message = "Failed to update the restaurant" });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access during restaurant update");
                return Unauthorized(new { message = "Invalid credentials provided for update" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while updating the restaurant: {register.RestaurantName}");
                return StatusCode(500, new { message = "An error occurred during update of the restaurant" });
            }
        }
    }
}
