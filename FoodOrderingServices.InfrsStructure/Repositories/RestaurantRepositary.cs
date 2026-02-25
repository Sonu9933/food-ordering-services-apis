using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.DTOs.Restaurant;
using FoodOrderingServices.Core.Entity;
using FoodOrderingServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingServices.Infrastructure.Repositories
{
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

        public Task<IEnumerable<Restaurant>> GetAllRestaurentsAsync()
        {
            return Task.FromResult(_applicationDbContext.Restaurants.AsEnumerable());
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
