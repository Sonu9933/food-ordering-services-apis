using FoodOrderingServices.Core.DTOs.Customer;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests
{
    /// <summary>
    /// Base class for integration tests providing common setup and utilities.
    /// </summary>
    public class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>
    {
        protected readonly HttpClient Client;
        private readonly IntegrationTestWebAppFactory _factory;

        // Route constants with API versioning
        protected const string RegisterRoute = "api/v1/CustomerAuth/register-consumer";
        protected const string LoginRoute = "api/v1/CustomerAuth/login-consumer";

        public IntegrationTestBase(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            Client = factory.CreateClient();
        }

        /// <summary>
        /// Registers a customer and authenticates with a JWT token.
        /// </summary>
        protected async Task RegisterAndAuthenticateAsync(string email, string password, string fullName)
        {
            // Register
            var registerRequest = new RegisterCustomerRequest
            {
                ConsumerName = fullName,
                Email = email,
                Password = password
            };

            var registerResponse = await Client.PostAsJsonAsync(RegisterRoute, registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            // Login to get token
            var loginRequest = new LoginCustomerRequest
            {
                Email = email,
                Password = password
            };

            var loginResponse = await Client.PostAsJsonAsync(LoginRoute, loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
            if (authResponse?.Token != null)
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
            }
        }

        /// <summary>
        /// Clears the authentication token from the HTTP client.
        /// </summary>
        protected void ClearAuthentication()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}