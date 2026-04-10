using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FoodOrderingServices.Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FoodOrderingServices.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.UseContentRoot(AppContext.BaseDirectory);

            base.ConfigureWebHost(builder);

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices(services =>
            {
                // Replace real DbContext with in-memory
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));

                // Replace Redis with in-memory distributed cache
                var cacheDescriptors = services
                    .Where(d => d.ServiceType == typeof(IDistributedCache))
                    .ToList();
                foreach (var d in cacheDescriptors)
                    services.Remove(d);

                services.AddDistributedMemoryCache();
            });
        }
    }
}