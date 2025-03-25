using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using System.Linq;

namespace back_end_tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockRepo;
        private readonly EventService _service;

        public EventServiceTests()
        {
            _mockRepo = new Mock<IEventRepository>();
            _service = new EventService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsEvents()
        {
            var mockEvents = new List<Event> { new Event { Title = "E1" }, new Event { Title = "E2" } };
            _mockRepo.Setup(repo => repo.GetAllEventsAsync()).ReturnsAsync(mockEvents);

            var result = await _service.GetAllEventsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEventByIdAsync_ReturnsCorrectEvent()
        {
            var ev = new Event { Id = 1, Title = "Test" };
            _mockRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(ev);

            var result = await _service.GetEventByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddEventAsync_CallsRepo()
        {
            var ev = new Event { Title = "New" };
            await _service.AddEventAsync(ev);
            _mockRepo.Verify(r => r.AddEventAsync(ev), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_CallsRepo()
        {
            var ev = new Event { Id = 2 };
            await _service.UpdateEventAsync(ev);
            _mockRepo.Verify(r => r.UpdateEventAsync(ev), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_CallsRepo()
        {
            await _service.DeleteEventAsync(99);
            _mockRepo.Verify(r => r.DeleteEventAsync(99), Times.Once);
        }
    }
}