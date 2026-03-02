
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Core.Contracts.Repositories
{
    /// <summary>
    /// Defines the contract for a repository that manages restaurant data, providing methods to retrieve, add, update,
    /// and delete restaurants.
    /// </summary>
    /// <remarks>This interface is intended for use in data access layers, allowing for asynchronous
    /// operations on restaurant entities. Implementations should ensure thread safety and handle any exceptions that
    /// may arise during data operations.</remarks>
    public interface IRestaurantRepositary
    {
            Task<IEnumerable<Restaurant>> GetAllRestaurentsAsync();
            Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId);
            Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest registerRequest);
            Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest registerRequest);
            Task<bool> DeleteRestaurentAsync(Guid restaurentId);
    }
}
