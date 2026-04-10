namespace FoodOrderingServices.Core.Contracts.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync();
    }
}
