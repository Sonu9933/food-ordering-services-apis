using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;

namespace FoodOrderingServices.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepositary _paymentRepositary;

        public PaymentService(IPaymentRepositary paymentRepositary)
        {
            _paymentRepositary = paymentRepositary;
        }

        public async Task<bool> ProcessPaymentAsync()
        {
            // Call repository and return hardcoded success
            return await _paymentRepositary.ProcessPaymentAsync();
        }
    }
}