using Asp.Versioning;
using FoodOrderingServices.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingServices.API.Controllers
{
    /// <summary>
    /// Exposes payment processing endpoints for the food ordering platform.
    /// </summary>
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiVersion(1)]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initialises a new instance of <see cref="PaymentController"/>.
        /// </summary>
        /// <param name="paymentService">
        /// The payment service responsible for orchestrating the payment transaction.
        /// </param>
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Initiates a payment transaction.
        /// </summary>
        /// <returns>
        /// <c>200 OK</c> with <c>{ "success": true/false }</c> indicating whether
        /// the payment was processed successfully.
        /// </returns>
        /// <response code="200">Payment processed. Check the <c>success</c> field for the outcome.</response>
        [HttpPost("process")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessPayment()
        {
            var result = await _paymentService.ProcessPaymentAsync();
            return Ok(new { Success = result });
        }
    }
}