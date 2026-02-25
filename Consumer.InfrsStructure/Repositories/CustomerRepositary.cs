using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Infrastructure.Data;

namespace Customer.Infrastructure.Repositories
{
    public class CustomerRepositary : ICustomerRepositary
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private IDistributedCache _distributedCache;

        public CustomerRepositary(ApplicationDbContext customerDbContext, IDistributedCache distributedCache) 
        {
            _applicationDbContext = customerDbContext;
            _distributedCache = distributedCache;
        }

        public async Task<FoodOrderingServices.Core.Entity.Customer?> LoginCustomerAsync(string email, string password)
        {
            var cacheCustomer = await _distributedCache.GetStringAsync(email);

            if (cacheCustomer != null)
            {
                var cachedCustomer = System.Text.Json.JsonSerializer.Deserialize<FoodOrderingServices.Core.Entity.Customer>(cacheCustomer);
                if (cachedCustomer != null && BCrypt.Net.BCrypt.Verify(password, cachedCustomer.PasswordHash))
                {
                    return cachedCustomer;
                }
            }

            var customer = await _applicationDbContext
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) { 
                return null;
            }

            customer.LastLogin = DateTime.UtcNow;
            _applicationDbContext.Customers.Update(customer);
            await _applicationDbContext.SaveChangesAsync();

            _distributedCache.SetString(email, System.Text.Json.JsonSerializer.Serialize(customer),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return customer;
        }

        public async Task<FoodOrderingServices.Core.Entity.Customer?> RegisterCustomerAsync(string customerName, 
            string email, 
            string password)
        {
            if (await _applicationDbContext.Customers.AnyAsync(c => c.Email == email))
            {
                return null;
            }

            var customer = new FoodOrderingServices.Core.Entity.Customer
            {
                CustomerId = new Guid(),
                CustomerName = customerName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLogin = null
            };

            await _applicationDbContext.Customers.AddAsync(customer);
            await _applicationDbContext.SaveChangesAsync();
            return customer;
        }
    }
}
