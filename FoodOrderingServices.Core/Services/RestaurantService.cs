using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepositary _restaurantRepositary;

        public RestaurantService(IRestaurantRepositary restaurantRepositary)
        {
            _restaurantRepositary = restaurantRepositary;
        }

        public async Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest restaurent)
        {
            var result = await _restaurantRepositary.AddRestaurentAsync(restaurent);

            if(result is null)
                return null;
            else
                return result;
        }

        public async Task<bool> DeleteRestaurentAsync(Guid restaurentId)
        {
            var result = await _restaurantRepositary.DeleteRestaurentAsync(restaurentId);

            if (result is false)
                return false;
            else
                return result;
        }

        public async Task<IEnumerable<Restaurant>?> GetAllRestaurentsAsync()
        {
            var result = await _restaurantRepositary.GetAllRestaurentsAsync();

            if (result is null)
                return null;
            else
                return result;
        }

        public async Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId)
        {
            var result = await _restaurantRepositary.GetRestaurentByIdAsync(restaurentId);

            if (result is null)
                return null;
            else
                return result;
        }

        public async Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest restaurent)
        {
            var result = await _restaurantRepositary.UpdateRestaurentAsync(restaurent);

            if (result is null)
                return null;
            else
                return result;
        }
    }
}
