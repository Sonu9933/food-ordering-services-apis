using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FoodOrderingServices.API.Controllers
{
    /// <summary>
    /// Controller for managing customer orders, including creating, updating, and retrieving order information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("limiting")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<RestaurantController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="orderService">The order service for handling business logic.</param>
        /// <param name="logger">The logger for recording application events.</param>
        public OrderController(
            IOrderService orderService,
            ILogger<RestaurantController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="orderRequest">The order registration details.</param>
        /// <returns>A message indicating successful registration or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpPost]
        [Route("place-order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderRequest orderRequest)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(orderRequest);
                if (result.OrderId != Guid.Empty)
                {
                    _logger.LogError("order placed successfullt and you order id is " + result.OrderId);
                    return Ok(new OrderSuccessResponse
                    {
                        ETA = result.ETA,
                        OrderId = result.OrderId,
                        OrderStatus = result.OrderStatus
                    });
                }
                else
                {
                    _logger.LogWarning("Order creation failed for request: {@OrderRequest}", orderRequest);
                    return BadRequest(new { message = "Order creation failed." });
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access");
                return Unauthorized(new { message = "Invalid token!" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while creating an order" + exception.Message);
                return StatusCode(500, new { message = "An error while creating an order" });
            }
        }

        /// <summary>
        /// Get order detail via an order id.
        /// </summary>
        /// <param name="orderId">The id of an order.</param>
        /// <returns>A message indicating order details or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpGet]
        [Route("get-order-detail-orderId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(orderId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("Order not found.");
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access");
                return Unauthorized(new { message = "Invalid token!" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while retrieving order details" + exception.Message);
                return StatusCode(500, new { message = "An error while retrieving order details" });
            }
        }

        /// <summary>
        /// Get order detail via an customer id.
        /// </summary>
        /// <param name="customerId">The id of an order.</param>
        /// <returns>A message indicating orders with details or an error message.</returns>
        /// <response code="200">Restaurant registered successfully.</response>
        /// <response code="400">Restaurant registration failed.</response>
        /// <response code="401">Unauthorized access - invalid credentials.</response>
        /// <response code="500">An unexpected error occurred during registration.</response>
        [HttpGet]
        [Route("get-order-detail-customerId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderByCustomerIdAsync(Guid customerId)
        {
            try
            {
                var result = await _orderService.GetOrdersByCustomerIdAsync(customerId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("Order/Orders not found.");
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(exception, "Unauthorized access");
                return Unauthorized(new { message = "Invalid token!" });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected error while retrieving order details" + exception.Message);
                return StatusCode(500, new { message = "An error while retrieving order details" });
            }
        }
    }
}
