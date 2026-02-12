using ConsumerEnpoints.Models;
using Customer.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly IntegrationTestWebAppFactory Factory;
        protected readonly CustomerDbContext Context;
        private readonly IServiceScope _scope;

        protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
        {
            Factory = factory;

            Client = factory.CreateClient();

            _scope = factory.Services.CreateScope();

            Context = _scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            CleanDatabase();
        }

        private void CleanDatabase()
        {
            // Remove all data (order matters due to foreign keys)
            Context.Customers.RemoveRange(Context.Customers);
            Context.SaveChanges();
        }


        public void Dispose()
        {
            _scope.Dispose();
        }

        protected async Task<T?> ReloadFromDb<T>(T entity) where T : class
        {
            Context.ChangeTracker.Clear();

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} doesn't have an Id property");

            var id = idProperty.GetValue(entity);

            return await Context.Set<T>().FindAsync(id);
        }

        protected async Task<AuthResponse> AuthenticateUserAsync(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await Client.PostAsJsonAsync("api/auth/login", loginRequest);

            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            // set token for subsequent requests
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

            return authResponse;
        }

        #region Additional Test Scaffolding

        protected async Task<AuthResponse> RegisterAndAuthenticateAsync(
            string? email = null,
            string? password = null,
            string? fullName = null)
        {
            email ??= $"test{Guid.NewGuid():N}@example.com";
            password ??= "SecurePass123!";
            fullName ??= "Test User";

            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = password,
                ConsumerName = fullName
            };

            var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            // Set token for subsequent requests
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

            return authResponse;
        }

        protected void ClearAuthentication()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }

        #endregion
    }
}
