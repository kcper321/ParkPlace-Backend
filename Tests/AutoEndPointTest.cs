using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ParkingAPI.Tests
{
    public class AutoEndPointTest
    {
        public async Task TestEndpoint_ParkingSpots()
        {
            var app = new WebApplicationFactory<Program>();
            var client = app.CreateClient();
            var response = await client.GetAsync("/api/parking/available-spots");
            Assert.Equal(200, (int)response.StatusCode);
        }
    }
}
