using FoodOrderingServices.Core.DTOs.Customer;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class CustomerAuthControllerIntegrationTests : IntegrationTestBase
    {
        public CustomerAuthControllerIntegrationTests(IntegrationTestWebAppFactory factory)
            : base(factory) { }


        [Fact]
        public async Task RegisterConsumer_ReturnsOk_WithValidRequest()
        {
            var request = BuildRegisterRequest("alice@test.com");

            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RegisterConsumer_ReturnsBadRequest_WithDuplicateEmail()
        {
            var request = BuildRegisterRequest("duplicate@test.com");
            await Client.PostAsJsonAsync(RegisterRoute, request);  // first registration

            var response = await Client.PostAsJsonAsync(RegisterRoute, request);  // duplicate

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterConsumer_ReturnsBadRequest_WhenConsumerNameExceedsMaxLength()
        {
            var request = new RegisterCustomerRequest
            {
                ConsumerName = "TooLongName",  // > 6 chars — violates [MaxLength(6)]
                Email        = "long@test.com",
                Password     = "SecurePass123!"
            };

            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task LoginConsumer_ReturnsOk_WithValidCredentials()
        {
            await RegisterAndAuthenticateAsync("login@test.com", "SecurePass123!", "Bob");
            ClearAuthentication();

            var loginRequest = new LoginCustomerRequest
            {
                Email    = "login@test.com",
                Password = "SecurePass123!"
            };

            var response = await Client.PostAsJsonAsync(LoginRoute, loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginConsumer_ResponseContainsToken_OnSuccess()
        {
            await RegisterAndAuthenticateAsync("token@test.com", "SecurePass123!", "Carol");
            ClearAuthentication();

            var loginRequest = new LoginCustomerRequest
            {
                Email    = "token@test.com",
                Password = "SecurePass123!"
            };

            var response     = await Client.PostAsJsonAsync(LoginRoute, loginRequest);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            Assert.NotNull(authResponse);
            Assert.False(string.IsNullOrEmpty(authResponse!.Token));
        }

        [Fact]
        public async Task LoginConsumer_ReturnsUnauthorized_WhenEmailNotFound()
        {
            var loginRequest = new LoginCustomerRequest
            {
                Email    = "nobody@test.com",
                Password = "SecurePass123!"
            };

            var response = await Client.PostAsJsonAsync(LoginRoute, loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private static RegisterCustomerRequest BuildRegisterRequest(string email) => new()
        {
            ConsumerName = "Alice",      // ≤ 6 chars
            Email        = email,
            Password     = "SecurePass123!"
        };
    }
}