using Microsoft.AspNetCore.Mvc;
using FoodOrderingServices.Core.Contracts.Services;

namespace FoodOrderingServices.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment()
        {
            var result = await _paymentService.ProcessPaymentAsync();
            return Ok(new { Success = result });
        }
    }
}