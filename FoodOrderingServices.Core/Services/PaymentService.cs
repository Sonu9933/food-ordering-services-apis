using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;

namespace FoodOrderingServices.Core.Services
{
    /// <summary>
    /// Handles payment business logic by delegating to the underlying payment repository.
    /// </summary>
    /// <remarks>
    /// Acts as the orchestration layer between the <see cref="PaymentController"/> and
    /// the data/gateway layer represented by <see cref="IPaymentRepositary"/>.
    /// Future enhancements may include pre-authorisation checks, fraud detection,
    /// or order-status updates before delegating to the repository.
    /// </remarks>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepositary _paymentRepositary;

        /// <summary>
        /// Initialises a new instance of <see cref="PaymentService"/> with the required repository.
        /// </summary>
        /// <param name="paymentRepositary">
        /// The repository responsible for persisting and processing payment transactions.
        /// </param>
        public PaymentService(IPaymentRepositary paymentRepositary)
        {
            _paymentRepositary = paymentRepositary;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Delegates directly to <see cref="IPaymentRepositary.ProcessPaymentAsync"/>.
        /// Add pre- or post-processing logic here (e.g., idempotency checks, event publishing)
        /// as the payment feature matures.
        /// </remarks>
        public async Task<bool> ProcessPaymentAsync()
        {
            return await _paymentRepositary.ProcessPaymentAsync();
        }
    }
}