using FoodOrderingServices.Core.Contracts.Repositories;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    public class PaymentRepositary : IPaymentRepositary
    {
        public Task<bool> ProcessPaymentAsync()
        {
            // Hardcoded success
            return Task.FromResult(true);
        }
    }
}
