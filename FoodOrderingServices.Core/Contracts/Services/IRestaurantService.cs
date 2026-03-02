using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Services
{
    /// <summary>
    /// Defines the contract for restaurant-related operations, including retrieval, addition, updating, and deletion of
    /// restaurant details.
    /// </summary>
    /// <remarks>This interface provides asynchronous methods for managing restaurant data. Implementations
    /// should ensure thread safety and handle potential exceptions appropriately.</remarks>
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDetailDTO>?> GetAllRestaurentsAsync();
        Task<RestaurantDetailDTO?> GetRestaurentByIdAsync(Guid restaurentId);
        Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest restaurent);
        Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest restaurent);
        Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
