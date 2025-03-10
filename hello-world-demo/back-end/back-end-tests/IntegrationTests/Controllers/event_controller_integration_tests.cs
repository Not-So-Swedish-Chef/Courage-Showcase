using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using back_end.Models;
using back_end;

namespace back_end_tests.IntegrationTests.Controllers
{
    public class event_controller_integration_tests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public event_controller_integration_tests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task create_event_returns_ok_for_valid_data()
        {
            // Arrange
            var event_data = new EventFormData
            {
                Title = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                ImageUrl = "https://example.com/image.jpg",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(2),
                CategoryId = "1",
                Price = 10,
                IsFree = false,
                Url = "https://example.com"
            };
            var content = new StringContent(JsonSerializer.Serialize(event_data), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/event/create", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}