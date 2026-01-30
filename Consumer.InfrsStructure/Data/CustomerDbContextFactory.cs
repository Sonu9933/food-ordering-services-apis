using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsumerEnpoints.Data
{
    public class CustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();

            // Use a dummy connection string for design-time migrations only
            // This won't actually connect to a database during migration generation
            optionsBuilder.UseSqlServer("Server=localhost;Database=ConsumerDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;");

            return new CustomerDbContext(optionsBuilder.Options);
        }
    }
}
