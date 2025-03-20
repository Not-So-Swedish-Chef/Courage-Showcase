using Moq;
using Xunit;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end_tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _eventService = new EventService(_mockEventRepository.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsListOfEvents()
        {
            // Arrange
            var mockEvents = new List<Event>
            {
                new Event { Title = "Event 1" },
                new Event { Title = "Event 2" }
            };
            _mockEventRepository.Setup(repo => repo.GetAllEventsAsync()).ReturnsAsync(mockEvents);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}