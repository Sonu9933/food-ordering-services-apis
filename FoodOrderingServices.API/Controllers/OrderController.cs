using FoodOrderingServices.Core.Contracts.Services;
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
    }
}
