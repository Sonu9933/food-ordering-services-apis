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

        public async Task<IEnumerable<RestaurantDetailDTO>?> GetAllRestaurentsAsync()
        {
            // Repository likely returns IEnumerable<Restaurant>; map each entity to DTO.
            var result = await _restaurantRepositary.GetAllRestaurentsAsync();

            if (result is null)
                return null;

            // Map entity collection to DTO collection
            return [.. result.Select(MapToDetailDto)];
        }

        public async Task<RestaurantDetailDTO?> GetRestaurentByIdAsync(Guid restaurentId)
        {
            var result = await _restaurantRepositary.GetRestaurentByIdAsync(restaurentId);

            if (result is null)
                return null;

            // Map single entity to DTO
            return MapToDetailDto(result);
        }

        public async Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest restaurent)
        {
            var result = await _restaurantRepositary.UpdateRestaurentAsync(restaurent);

            if (result is null)
                return null;
            else
                return result;
        }

        // Helper mapping method: entity -> DTO
        private static RestaurantDetailDTO MapToDetailDto(Restaurant entity)
        {
            if (entity == null)
                return null!;

            return new RestaurantDetailDTO
            {
                RestaurantID = entity.RestaurantID,
                RestaurantName = entity.RestaurantName,
                Location = entity.Location,
                ContactNumber = entity.ContactNumber,
            };
        }
    }
}
