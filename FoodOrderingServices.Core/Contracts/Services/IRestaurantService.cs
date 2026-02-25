using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>?> GetAllRestaurentsAsync();
        Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId);
        Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest restaurent);
        Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest restaurent);
        Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
