using FoodOrderingServices.Core.DTOs.Restaurant;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class RestaurantControllerIntegrationTests : IntegrationTestBase
    {
        private const string BaseRoute     = "api/Restaurant";
        private const string GetAllRoute   = $"{BaseRoute}/get-all-restaurents";
        private const string RegisterRoute = $"{BaseRoute}/register-restaurant";
        private const string UpdateRoute   = $"{BaseRoute}/update-restaurant";

        public RestaurantControllerIntegrationTests(IntegrationTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task GetAllRestaurents_ReturnsOk_WhenNoRestaurantsExist()
        {
            var response = await Client.GetAsync(GetAllRoute);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllRestaurents_ReturnsRegisteredRestaurant()
        {
            await RegisterRestaurantAsync("Sushi Palace", "1111111111");

            var response     = await Client.GetAsync(GetAllRoute);
            var restaurants  = await response.Content
                .ReadFromJsonAsync<IEnumerable<RestaurantDetailDTO>>();

            Assert.NotNull(restaurants);
            Assert.Contains(restaurants!, r => r.RestaurantName == "Sushi Palace");
        }


        [Fact]
        public async Task RegisterRestaurant_ReturnsOk_WithValidRequest()
        {
            var request  = BuildRequest("Burger Joint", "2222222222");
            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RegisterRestaurant_ReturnsBadRequest_WhenDuplicateNameExists()
        {
            await RegisterRestaurantAsync("Duplicate Cafe", "3333333333");

            var request  = BuildRequest("Duplicate Cafe", "4444444444");
            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task UpdateRestaurant_ReturnsOk_WhenRestaurantExists()
        {
            var original = BuildRequest("Update Me", "5555555555");
            await Client.PostAsJsonAsync(RegisterRoute, original);

            var updateRequest = new AddRestaurantRequest
            {
                RestaurantName = "Update Me",
                Location       = "Paris",
                ContactNumber  = "5555555555"
            };

            var response = await Client.PostAsJsonAsync(UpdateRoute, updateRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRestaurant_ReturnsBadRequest_WhenRestaurantDoesNotExist()
        {
            var request  = BuildRequest("Ghost Cafe", "9999999999");
            var response = await Client.PostAsJsonAsync(UpdateRoute, request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task RegisterRestaurantAsync(string name, string phone)
        {
            var response = await Client.PostAsJsonAsync(RegisterRoute, BuildRequest(name, phone));
            response.EnsureSuccessStatusCode();
        }

        private static AddRestaurantRequest BuildRequest(string name, string phone) => new()
        {
            RestaurantName = name,
            Location       = "London",
            ContactNumber  = phone
        };
    }
}