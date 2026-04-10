using FoodOrderingServices.Core.Contracts.Repositories;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    /// <summary>
    /// Stub implementation of <see cref="IPaymentRepositary"/> that always reports payment success.
    /// </summary>
    /// <remarks>
    /// This class is a placeholder for a real payment gateway integration (e.g., Stripe, PayPal).
    /// It satisfies the interface contract during development so the rest of the payment
    /// pipeline can be built and tested end-to-end without a live payment provider.
    /// Replace the body of <see cref="ProcessPaymentAsync"/> with actual gateway calls
    /// and transaction persistence before releasing to production.
    /// </remarks>
    public class PaymentRepositary : IPaymentRepositary
    {
        /// <summary>
        /// Simulates processing a payment by immediately returning <see langword="true"/>.
        /// </summary>
        /// <returns>
        /// A completed <see cref="Task{Boolean}"/> whose result is always <see langword="true"/>.
        /// </returns>
        /// <remarks>
        /// ⚠️ Stub implementation — does not communicate with any real payment provider.
        /// </remarks>
        public Task<bool> ProcessPaymentAsync()
        {
            // TODO: Replace with a real payment gateway call and persist the transaction record.
            return Task.FromResult(true);
        }
    }
}
