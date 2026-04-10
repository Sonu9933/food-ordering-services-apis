namespace FoodOrderingServices.Core.Contracts.Repositories
{
    public interface IPaymentRepositary
    {
        Task<bool> ProcessPaymentAsync();
    }
}
