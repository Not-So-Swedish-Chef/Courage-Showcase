using back_end.Models;
using back_end.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace back_end_tests.Repositories
{
    public class EventRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly EventRepository _repository;
        private readonly Mock<ILogger<EventRepository>> _mockLogger;

        public EventRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<EventRepository>>();
            _repository = new EventRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddEventAsync_AddsEvent()
        {
            var eventItem = new Event { Title = "Test Event", Location = "Online", HostId = 1 };

            await _repository.AddEventAsync(eventItem);

            var storedEvent = await _context.Events.FindAsync(eventItem.Id);
            Assert.NotNull(storedEvent);
            Assert.Equal("Test Event", storedEvent.Title);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsAllEvents()
        {
            _context.Events.AddRange(
                new Event { Title = "Event 1", HostId = 1 },
                new Event { Title = "Event 2", HostId = 1 }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllEventsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, ((List<Event>)result).Count);
        }

        [Fact]
        public async Task GetEventByIdAsync_ReturnsCorrectEvent()
        {
            var eventItem = new Event { Title = "Unique Event", HostId = 1 };
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            var result = await _repository.GetEventByIdAsync(eventItem.Id);

            Assert.NotNull(result);
            Assert.Equal("Unique Event", result.Title);
        }

        [Fact]
        public async Task UpdateEventAsync_UpdatesEvent()
        {
            var eventItem = new Event { Title = "Original", HostId = 1 };
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            eventItem.Title = "Updated";
            await _repository.UpdateEventAsync(eventItem);

            var updated = await _context.Events.FindAsync(eventItem.Id);
            Assert.Equal("Updated", updated.Title);
        }

        [Fact]
        public async Task DeleteEventAsync_RemovesEvent()
        {
            var eventItem = new Event { Title = "To Delete", HostId = 1 };
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            await _repository.DeleteEventAsync(eventItem.Id);

            var result = await _context.Events.FindAsync(eventItem.Id);
            Assert.Null(result);
        }
    }
}
