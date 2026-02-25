using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDetailDTO>?> GetAllRestaurentsAsync();
        Task<RestaurantDetailDTO?> GetRestaurentByIdAsync(Guid restaurentId);
        Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest restaurent);
        Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest restaurent);
        Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
