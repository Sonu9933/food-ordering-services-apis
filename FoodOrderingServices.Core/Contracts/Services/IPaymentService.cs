namespace FoodOrderingServices.Core.Contracts.Services
{
    /// <summary>
    /// Defines the business-logic contract for payment operations.
    /// </summary>
    /// <remarks>
    /// Abstracts the payment processing pipeline so the API layer
    /// has no knowledge of the underlying payment mechanism.
    /// The current implementation is a stub that always succeeds;
    /// a real implementation would integrate with a payment gateway (e.g., Stripe, PayPal).
    /// </remarks>
    public interface IPaymentService
    {
        /// <summary>
        /// Initiates a payment transaction asynchronously.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the payment was processed successfully;
        /// <see langword="false"/> if the payment was declined or failed.
        /// </returns>
        Task<bool> ProcessPaymentAsync();
    }
}
