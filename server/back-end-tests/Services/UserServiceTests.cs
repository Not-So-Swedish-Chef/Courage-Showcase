using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using back_end.Models;
using back_end.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _service;
        private readonly Mock<ILogger<UserService>> _mockLogger;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<UserService>>();
            _service = new UserService(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Concert", Location = "NYC", Price = 100, HostId = 10 },
                new Event { Id = 2, Title = "Conference", Location = "SF", Price = 200, HostId = 20 },
                new Event { Id = 3, Title = "Meetup", Location = "Chicago", Price = 0, HostId = 30 }
            };
            _context.Events.AddRange(events);

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice@example.com",
                    UserName = "alice@example.com",
                    SavedEvents = new List<Event> { events[0] }
                },
                new User
                {
                    Id = 2,
                    FirstName = "Bob",
                    LastName = "Jones",
                    Email = "bob@example.com",
                    UserName = "bob@example.com"
                }
            };
            _context.Users.AddRange(users);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetSavedEventsAsync_ShouldReturnSavedEvents()
        {
            var result = await _service.GetSavedEventsAsync(1);

            Assert.Single(result);
            Assert.Equal("Concert", result.First().Title);
        }

        [Fact]
        public async Task GetSavedEventsAsync_UserWithoutEvents_ShouldReturnEmpty()
        {
            var result = await _service.GetSavedEventsAsync(2);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSavedEventsAsync_NonExistentUser_ShouldReturnEmpty()
        {
            var result = await _service.GetSavedEventsAsync(999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task SaveEventAsync_ShouldAddEventToUser()
        {
            var success = await _service.SaveEventAsync(2, 2);

            Assert.True(success);

            var user = await _context.Users.Include(u => u.SavedEvents).FirstAsync(u => u.Id == 2);
            Assert.Single(user.SavedEvents);
            Assert.Equal(2, user.SavedEvents.First().Id);
        }

        [Fact]
        public async Task SaveEventAsync_EventAlreadySaved_ShouldNotDuplicate()
        {
            var success = await _service.SaveEventAsync(1, 1);

            Assert.True(success);

            var user = await _context.Users.Include(u => u.SavedEvents).FirstAsync(u => u.Id == 1);
            Assert.Single(user.SavedEvents); // still 1
        }

        [Fact]
        public async Task SaveEventAsync_NonExistentUser_ShouldReturnFalse()
        {
            var result = await _service.SaveEventAsync(999, 1);

            Assert.False(result);
        }

        [Fact]
        public async Task SaveEventAsync_NonExistentEvent_ShouldReturnFalse()
        {
            var result = await _service.SaveEventAsync(1, 999);

            Assert.False(result);
        }

        [Fact]
        public async Task RemoveSavedEventAsync_ShouldRemoveEventFromUser()
        {
            var success = await _service.RemoveSavedEventAsync(1, 1);

            Assert.True(success);

            var user = await _context.Users.Include(u => u.SavedEvents).FirstAsync(u => u.Id == 1);
            Assert.Empty(user.SavedEvents);
        }

        [Fact]
        public async Task RemoveSavedEventAsync_EventNotInUser_ShouldStillReturnTrue()
        {
            var success = await _service.RemoveSavedEventAsync(2, 1);

            Assert.True(success);

            var user = await _context.Users.Include(u => u.SavedEvents).FirstAsync(u => u.Id == 2);
            Assert.Empty(user.SavedEvents);
        }

        [Fact]
        public async Task RemoveSavedEventAsync_NonExistentUser_ShouldReturnFalse()
        {
            var result = await _service.RemoveSavedEventAsync(999, 1);

            Assert.False(result);
        }

        [Fact]
        public async Task RemoveSavedEventAsync_NonExistentEvent_ShouldReturnFalse()
        {
            var result = await _service.RemoveSavedEventAsync(1, 999);

            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
