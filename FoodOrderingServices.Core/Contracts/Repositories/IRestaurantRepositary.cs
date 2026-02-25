
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Repositories
{
    public interface IRestaurantRepositary
    {
            Task<IEnumerable<Restaurant>> GetAllRestaurentsAsync();
            Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId);
            Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest registerRequest);
            Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest registerRequest);
            Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
