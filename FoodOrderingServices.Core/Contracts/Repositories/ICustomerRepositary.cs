namespace FoodOrderingServices.Core.Contracts.Repositories
{
    /// <summary>
    /// Defines methods for authenticating and registering customers asynchronously in the system.
    /// </summary>
    /// <remarks>Implementations of this interface should validate input parameters and ensure secure handling of
    /// customer credentials. Methods return a customer entity if the operation succeeds; otherwise, they return null. These
    /// operations are typically used to support user login and registration workflows in client applications.</remarks>
    public interface ICustomerRepositary
    {
        Task<Entity.Customer?> LoginCustomerAsync(string email, string password);
        Task<Entity.Customer?> RegisterCustomerAsync(string customerName, string email, string password);
    }
}
