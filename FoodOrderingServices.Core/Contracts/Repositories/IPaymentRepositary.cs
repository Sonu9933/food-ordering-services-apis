namespace FoodOrderingServices.Core.Contracts.Repositories
{
    /// <summary>
    /// Defines the data-access contract for payment persistence operations.
    /// </summary>
    /// <remarks>
    /// Separates payment storage concerns from business logic.
    /// The current implementation is a hardcoded stub returning <see langword="true"/>;
    /// a production implementation would persist transaction records and communicate
    /// with an external payment provider.
    /// </remarks>
    public interface IPaymentRepositary
    {
        /// <summary>
        /// Records and processes a payment transaction asynchronously.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the transaction was recorded and approved;
        /// <see langword="false"/> if recording failed or the transaction was rejected.
        /// </returns>
        Task<bool> ProcessPaymentAsync();
    }
}
