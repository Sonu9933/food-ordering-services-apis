using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    /// <summary>
    /// Provides methods for managing restaurant data, including adding, deleting, retrieving, and updating restaurant
    /// information.
    /// </summary>
    /// <remarks>This repository interacts with the underlying database context to perform CRUD operations on
    /// restaurant entities. It ensures that restaurant names and contact numbers are unique when adding new
    /// restaurants. The methods are asynchronous and return tasks to allow for non-blocking operations.</remarks>
    public class RestaurantRepositary : IRestaurantRepositary
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public RestaurantRepositary(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Restaurant?> AddRestaurentAsync(AddRestaurantRequest registerRequest)
        {
            if(await _applicationDbContext
                .Restaurants
                .AnyAsync(r => r.RestaurantName == registerRequest.RestaurantName || r.ContactNumber == registerRequest.ContactNumber))
            {
                return null;
            }

            var restaurant = new Restaurant
            {
                RestaurantID = Guid.NewGuid(),
                RestaurantName = registerRequest.RestaurantName,
                ContactNumber = registerRequest.ContactNumber,
                Location = registerRequest.Location,
                CreatedAt = DateTime.UtcNow
            };

            await _applicationDbContext.Restaurants.AddAsync(restaurant);
            await _applicationDbContext.SaveChangesAsync();

            return restaurant;
        }

        public async Task<bool> DeleteRestaurentAsync(Guid restaurentId)
        {
            if (await _applicationDbContext.Restaurants.AnyAsync(r => r.RestaurantID == restaurentId))
            {
                return false;
            }

            var restaurant = _applicationDbContext.Restaurants.Where(r => r.RestaurantID == restaurentId).FirstOrDefault();
            if (restaurant != null)
            {
                _applicationDbContext.Restaurants.Remove(restaurant);
                _applicationDbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurentsAsync()
        {
            var restaurants = await _applicationDbContext.Restaurants.ToListAsync();

            return restaurants.Count > 0 ? restaurants : Enumerable.Empty<Restaurant>();
        }

        public async Task<Restaurant?> GetRestaurentByIdAsync(Guid restaurentId)
        {
            if(await _applicationDbContext.Restaurants.AnyAsync(r => r.RestaurantID == restaurentId))
            {
                return _applicationDbContext.Restaurants.Where(r => r.RestaurantID == restaurentId).FirstOrDefault();
            }

            return null;
        }

        public async Task<Restaurant?> UpdateRestaurentAsync(AddRestaurantRequest registerRequest)
        {
            var restaurant = await _applicationDbContext
                .Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RestaurantName == registerRequest.RestaurantName || r.ContactNumber == registerRequest.ContactNumber);

            if (restaurant == null)
            {
                return null;
            }

            restaurant.Location = registerRequest.Location;
            restaurant.ContactNumber = registerRequest.ContactNumber;
            restaurant.RestaurantName = registerRequest.RestaurantName;
            restaurant.UpdatedAt = DateTime.UtcNow; 

            _applicationDbContext.Restaurants.Update(restaurant);
            await _applicationDbContext.SaveChangesAsync();

            return restaurant;
        }
    }
}
