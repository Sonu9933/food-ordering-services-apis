namespace Customer.Core.Contracts.Repositories
{
    public interface ICustomerRepositary
    {
        Task<Entity.Customer?> LoginCustomerAsync(string email, string password);
        Task<Entity.Customer?> RegisterCustomerAsync(string customerName, string email, string password);
    }
}
