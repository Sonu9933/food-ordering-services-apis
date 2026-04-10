using FoodOrderingServices.Core.DTOs.Customer;
using FoodOrderingServices.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly IntegrationTestWebAppFactory Factory;
        protected readonly ApplicationDbContext Context;
        private readonly IServiceScope _scope;

        // Correct API route constants
        protected const string RegisterRoute   = "api/CustomerAuth/register-consumer";
        protected const string LoginRoute      = "api/CustomerAuth/login-consumer";

        protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
        {
            Factory = factory;
            Client  = factory.CreateClient();
            _scope  = factory.Services.CreateScope();
            Context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            CleanDatabase();
        }

        private void CleanDatabase()
        {
            // Order matters due to foreign keys
            Context.OrderDetails.RemoveRange(Context.OrderDetails);
            Context.Orders.RemoveRange(Context.Orders);
            Context.MenuItems.RemoveRange(Context.MenuItems);
            Context.Restaurants.RemoveRange(Context.Restaurants);
            Context.Customers.RemoveRange(Context.Customers);
            Context.SaveChanges();
        }

        public void Dispose() => _scope.Dispose();

        protected async Task<T?> ReloadFromDb<T>(T entity) where T : class
        {
            Context.ChangeTracker.Clear();

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} doesn't have an Id property");

            var id = idProperty.GetValue(entity);
            return await Context.Set<T>().FindAsync(id);
        }

        protected async Task<AuthenticationResponse> AuthenticateUserAsync(string email, string password)
        {
            var loginRequest = new LoginCustomerRequest { Email = email, Password = password };

            var response = await Client.PostAsJsonAsync(LoginRoute, loginRequest);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

            return authResponse;
        }

        #region Additional Test Scaffolding

        protected async Task<AuthenticationResponse> RegisterAndAuthenticateAsync(
            string? email    = null,
            string? password = null,
            string? fullName = null)
        {
            // NOTE: ConsumerName has [MaxLength(6)] — keep name ≤ 6 chars
            email    ??= $"t{Guid.NewGuid():N}"[..12] + "@test.com";
            password ??= "SecurePass123!";
            fullName ??= "Alice";  // ≤ 6 chars to pass MaxLength(6) validation

            var registerRequest = new RegisterCustomerRequest
            {
                Email        = email,
                Password     = password,
                ConsumerName = fullName
            };

            var registerResponse = await Client.PostAsJsonAsync(RegisterRoute, registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            // Register endpoint only returns a message — must call login separately for the token
            return await AuthenticateUserAsync(email, password);
        }

        protected void ClearAuthentication()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }

        #endregion
    }
}
