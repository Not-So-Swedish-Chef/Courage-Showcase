using Xunit;
using Moq;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

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

        private Event CreateSampleEvent(int id = 1, int hostId = 100) =>
            new Event
            {
                Id = id,
                Title = "Sample Event",
                Location = "NYC",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(2),
                Price = 50,
                HostId = hostId
            };

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnEvents()
        {
            var events = new List<Event> { CreateSampleEvent(), CreateSampleEvent(2) };
            _mockRepo.Setup(r => r.GetAllEventsAsync()).ReturnsAsync(events);

            var result = await _service.GetAllEventsAsync();

            Assert.Equal(2, ((List<Event>)result).Count);
        }

        [Fact]
        public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
        {
            var evt = CreateSampleEvent();
            _mockRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(evt);

            var result = await _service.GetEventByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetEventByIdAsync_WithInvalidId_ShouldThrowDataException()
        {
            _mockRepo.Setup(r => r.GetEventByIdAsync(999)).ReturnsAsync((Event?)null);

            await Assert.ThrowsAsync<DataException>(() => _service.GetEventByIdAsync(999));
        }

        [Fact]
        public async Task AddEventAsync_ShouldCallRepository()
        {
            var evt = CreateSampleEvent();

            await _service.AddEventAsync(evt);

            _mockRepo.Verify(r => r.AddEventAsync(evt), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_WithAuthorizedUser_ShouldUpdate()
        {
            var evt = CreateSampleEvent();
            _mockRepo.Setup(r => r.GetEventByIdAsync(evt.Id)).ReturnsAsync(evt);

            await _service.UpdateEventAsync(evt, evt.HostId.ToString());

            _mockRepo.Verify(r => r.UpdateEventAsync(evt), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_WhenEventNotFound_ShouldThrowDataException()
        {
            var evt = CreateSampleEvent();
            _mockRepo.Setup(r => r.GetEventByIdAsync(evt.Id)).ReturnsAsync((Event?)null);

            await Assert.ThrowsAsync<DataException>(
                () => _service.UpdateEventAsync(evt, evt.HostId.ToString()));
        }

        [Fact]
        public async Task DeleteEventAsync_WithAuthorizedUser_ShouldDelete()
        {
            var evt = CreateSampleEvent();
            _mockRepo.Setup(r => r.GetEventByIdAsync(evt.Id)).ReturnsAsync(evt);

            await _service.DeleteEventAsync(evt.Id, evt.HostId.ToString());

            _mockRepo.Verify(r => r.DeleteEventAsync(evt.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_WhenEventNotFound_ShouldThrowDataException()
        {
            _mockRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync((Event?)null);

            await Assert.ThrowsAsync<DataException>(
                () => _service.DeleteEventAsync(1, "100"));
        }

        [Fact]
        public async Task SearchEventsAsync_ShouldReturnResults()
        {
            var events = new List<Event> { CreateSampleEvent(), CreateSampleEvent(2) };
            _mockRepo.Setup(r => r.SearchEventsAsync("Sample", null, null, null, null))
                     .ReturnsAsync(events);

            var result = await _service.SearchEventsAsync("Sample");

            Assert.Equal(2, ((List<Event>)result).Count);
        }
    }
}
