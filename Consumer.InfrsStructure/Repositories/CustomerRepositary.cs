using Customer.Infrastructure.Data;
using Customer.Core.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Customer.Infrastructure.Repositories
{
    public class CustomerRepositary : ICustomerRepositary
    {
        private readonly CustomerDbContext customerDbContext;
        private IDistributedCache distributedCache { get; }

        public CustomerRepositary(CustomerDbContext customerDbContext, IDistributedCache distributedCache) 
        {
            this.customerDbContext = customerDbContext;
            this.distributedCache = distributedCache;
        }

        public async Task<Core.Entity.Customer?> LoginCustomerAsync(string email, string password)
        {
            var cacheCustomer = await distributedCache.GetStringAsync(email);

            if (cacheCustomer != null)
            {
                var cachedCustomer = System.Text.Json.JsonSerializer.Deserialize<Core.Entity.Customer>(cacheCustomer);
                if (cachedCustomer != null && BCrypt.Net.BCrypt.Verify(password, cachedCustomer.PasswordHash))
                {
                    return cachedCustomer;
                }
            }

            var customer = await customerDbContext
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) { 
                return null;
            }

            customer.LastLogin = DateTime.UtcNow;
            customerDbContext.Customers.Update(customer);
            await customerDbContext.SaveChangesAsync();

            distributedCache.SetString(email, System.Text.Json.JsonSerializer.Serialize(customer),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return customer;
        }

        public async Task<Core.Entity.Customer?> RegisterCustomerAsync(string customerName, 
            string email, 
            string password)
        {
            if (await customerDbContext.Customers.AnyAsync(c => c.Email == email))
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

            await customerDbContext.Customers.AddAsync(customer);
            await customerDbContext.SaveChangesAsync();
            return customer;
        }
    }
}
