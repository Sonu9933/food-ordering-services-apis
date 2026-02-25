using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>?> GetAllRestaurentsAsync();
        Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId);
        Task<Restaurant?> AddRestaurentAsync(RegisterRequest restaurent);
        Task<Restaurant?> UpdateRestaurentAsync(RegisterRequest restaurent);
        Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
