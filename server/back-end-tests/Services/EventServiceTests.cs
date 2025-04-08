using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace back_end_tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockRepo;
        private readonly Mock<ILogger<EventService>> _mockLogger;
        private readonly EventService _service;

        public EventServiceTests()
        {
            _mockRepo = new Mock<IEventRepository>();
            _mockLogger = new Mock<ILogger<EventService>>();
            _service = new EventService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsEvents()
        {
            // Arrange
            var events = new List<Event> { new Event { Title = "Test Event" } };
            _mockRepo.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(events);

            // Act
            var result = await _service.GetAllEventsAsync();

            // Assert
            Assert.Equal(events, result);
        }

        [Fact]
        public async Task GetEventByIdAsync_ReturnsEvent_WhenExists()
        {
            var eventItem = new Event { Id = 1, Title = "Test" };
            _mockRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventItem);

            var result = await _service.GetEventByIdAsync(1);

            Assert.Equal(eventItem, result);
        }

        [Fact]
        public async Task GetEventByIdAsync_Throws_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetEventByIdAsync(2)).ReturnsAsync((Event)null);

            await Assert.ThrowsAsync<DataException>(() => _service.GetEventByIdAsync(2));
        }

        [Fact]
        public async Task AddEventAsync_CallsRepository()
        {
            var newEvent = new Event { Title = "New" };

            await _service.AddEventAsync(newEvent);

            _mockRepo.Verify(r => r.AddEventAsync(newEvent), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_Throws_WhenUserUnauthorized()
        {
            var ev = new Event { Id = 3, HostId = 44 };
            _mockRepo.Setup(r => r.GetEventByIdAsync(3)).ReturnsAsync(ev);

            var ex = await Assert.ThrowsAsync<DataException>(() => _service.UpdateEventAsync(ev, "99"));
            Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
            Assert.Equal("You are not authorized to update this event.", ex.InnerException?.Message);
        }

        [Fact]
        public async Task DeleteEventAsync_Throws_WhenUserUnauthorized()
        {
            var ev = new Event { Id = 10, HostId = 55 };
            _mockRepo.Setup(r => r.GetEventByIdAsync(10)).ReturnsAsync(ev);

            var ex = await Assert.ThrowsAsync<DataException>(() => _service.DeleteEventAsync(10, "99"));
            Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
            Assert.Equal("You are not authorized to delete this event.", ex.InnerException?.Message);
        }

        [Fact]
        public async Task DeleteEventAsync_Deletes_WhenAuthorized()
        {
            var ev = new Event { Id = 10, HostId = 55 };
            _mockRepo.Setup(r => r.GetEventByIdAsync(10)).ReturnsAsync(ev);

            await _service.DeleteEventAsync(10, "55");

            _mockRepo.Verify(r => r.DeleteEventAsync(10), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_Updates_WhenAuthorized()
        {
            var ev = new Event { Id = 3, HostId = 44 };
            _mockRepo.Setup(r => r.GetEventByIdAsync(3)).ReturnsAsync(ev);

            await _service.UpdateEventAsync(ev, "44");

            _mockRepo.Verify(r => r.UpdateEventAsync(ev), Times.Once);
        }
    }
}
