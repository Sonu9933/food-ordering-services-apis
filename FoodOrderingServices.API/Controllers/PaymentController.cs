using Microsoft.AspNetCore.Mvc;
using FoodOrderingServices.Core.Contracts.Services;

namespace FoodOrderingServices.API.Controllers
{
    /// <summary>
    /// Exposes payment processing endpoints for the food ordering platform.
    /// </summary>
    /// <remarks>
    /// The current implementation uses a stub service that always returns success.
    /// When a real payment gateway is integrated, this controller will support
    /// additional endpoints such as refunds, payment status queries, and webhook callbacks.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
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
        /// <remarks>
        /// ⚠️ Currently a stub — always returns <c>{ "success": true }</c>.
        /// A future version will accept a payment request body (amount, currency,
        /// payment method token) and integrate with a real payment gateway.
        /// </remarks>
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