using Customer.Infrastructure.Data;
using Customer.Core.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Repositories
{
    public class CustomerRepositary : ICustomerRepositary
    {
        private readonly CustomerDbContext customerDbContext;

        public CustomerRepositary(CustomerDbContext customerDbContext) 
        {
            this.customerDbContext = customerDbContext;
        } 

        public async Task<Core.Entity.Customer?> LoginCustomerAsync(string email, string password)
        {
            var customer = await customerDbContext
                .Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) { 
                return null;
            }

            customer.LastLogin = DateTime.UtcNow;
            customerDbContext.Customers.Update(customer);
            await customerDbContext.SaveChangesAsync();

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
