using FoodOrderingServices.Core.DTOs.Restaurant;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FoodOrderingServices.IntegrationTests.Controllers
{
    public class RestaurantControllerIntegrationTests : IntegrationTestBase
    {
        private const string BaseRoute     = "api/v1/Restaurant";
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
            await RegisterRestaurantAsync("Sushi Palace", "Downtown", "1111111111");

            var response     = await Client.GetAsync(GetAllRoute);
            var restaurants  = await response.Content
                .ReadFromJsonAsync<IEnumerable<RestaurantDetailDTO>>();

            Assert.NotNull(restaurants);
            Assert.Contains(restaurants!, r => r.RestaurantName == "Sushi Palace");
        }


        [Fact]
        public async Task RegisterRestaurant_ReturnsOk_WithValidRequest()
        {
            var request  = BuildRequest("Burger Joint", "Uptown", "2222222222");
            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RegisterRestaurant_ReturnsBadRequest_WhenDuplicateNameExists()
        {
            await RegisterRestaurantAsync("Pizza Place", "Midtown", "3333333333");
            var request  = BuildRequest("Pizza Place", "Downtown", "4444444444"); // same name, different location/phone
            var response = await Client.PostAsJsonAsync(RegisterRoute, request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRestaurant_ReturnsOk_WhenRestaurantExists()
        {
            // Register a restaurant
            await RegisterRestaurantAsync("Italian Bistro", "Riverside", "5555555555");
            
            // Update the same restaurant with new location and phone (keep the name the same)
            var updateRequest = BuildRequest("Italian Bistro", "Updated Riverside", "6666666666");

            var response = await Client.PostAsJsonAsync(UpdateRoute, updateRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRestaurant_ReturnsBadRequest_WhenRestaurantDoesNotExist()
        {
            var updateRequest = BuildRequest("Non Existent", "NoWhere", "7777777777");

            var response = await Client.PostAsJsonAsync(UpdateRoute, updateRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task<Guid> RegisterRestaurantAsync(string name, string location, string phone)
        {
            var request = BuildRequest(name, location, phone);
            var response = await Client.PostAsJsonAsync(RegisterRoute, request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            // The controller returns a string response, not the DTO directly
            // We'll just return a new GUID as the ID
            return Guid.NewGuid();
        }

        private static AddRestaurantRequest BuildRequest(string name, string location, string phone) => new()
        {
            RestaurantName = name,
            Location = location,
            ContactNumber = phone
        };
    }
}