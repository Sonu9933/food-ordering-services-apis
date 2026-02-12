using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Customer.Infrastructure.Data;

namespace FoodOrderingServices.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Optional
            // This sets ASPNETCORE_ENVIRONMENT to "Testing"
            // Can use this in Program.cs to configure test-specific behavior
            builder.UseEnvironment("Testing");
            builder.UseContentRoot(AppContext.BaseDirectory);

            // Call the base method to ensure default configuration
            base.ConfigureWebHost(builder);

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);
            });

            // Override services for testing
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CustomerDbContext>));

                if (descriptor != null)
                {
                    // because we don't want tests hitting prod database
                    services.Remove(descriptor);
                }

                // Step 2: Register test database (in-memory database)
                services.AddDbContext<CustomerDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);

                });
            });
        }
    }
}