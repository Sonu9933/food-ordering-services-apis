using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Infrastructure.Data;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    /// <summary>
    /// Provides methods for managing customer data, including login and registration functionalities.
    /// </summary>
    /// <remarks>This repository interacts with both a database context and a distributed cache to optimize
    /// customer login operations. It ensures that customer data is cached for quick access and updates the last login
    /// time upon successful login.</remarks>
    public class CustomerRepositary : ICustomerRepositary
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CustomerRepositary> _logger;

        public CustomerRepositary(
            ApplicationDbContext customerDbContext,
            IDistributedCache distributedCache,
            ILogger<CustomerRepositary> logger)
        {
            _applicationDbContext = customerDbContext;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<Core.Entity.Customer?> LoginCustomerAsync(string email, string password)
        {
            // Attempt cache lookup — fall back silently if Redis is unavailable
            try
            {
                var cacheCustomer = await _distributedCache.GetStringAsync(email);
                if (cacheCustomer != null)
                {
                    var cachedCustomer = System.Text.Json.JsonSerializer.Deserialize<Core.Entity.Customer>(cacheCustomer);
                    if (cachedCustomer != null && BCrypt.Net.BCrypt.Verify(password, cachedCustomer.PasswordHash))
                    {
                        return cachedCustomer;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable for key '{Email}'. Falling back to database.", email);
            }

            // Database fallback
            var customer = await _applicationDbContext
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null || !BCrypt.Net.BCrypt.Verify(password, customer.PasswordHash))
            {
                return null;
            }

            customer.LastLogin = DateTime.UtcNow;
            _applicationDbContext.Customers.Update(customer);
            await _applicationDbContext.SaveChangesAsync();

            // Attempt to repopulate cache — ignore failures
            try
            {
                await _distributedCache.SetStringAsync(
                    email,
                    System.Text.Json.JsonSerializer.Serialize(customer),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable. Could not cache customer '{Email}'.", email);
            }

            return customer;
        }

        public async Task<Core.Entity.Customer?> RegisterCustomerAsync(string customerName,
            string email,
            string password)
        {
            if (await _applicationDbContext.Customers.AnyAsync(c => c.Email == email))
            {
                return null;
            }

            var customer = new Core.Entity.Customer
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
