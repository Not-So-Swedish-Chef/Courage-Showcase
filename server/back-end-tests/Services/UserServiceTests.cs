using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using back_end.Enums;
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

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var result = await _service.GetAllUsersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SuspendUserAsync_ShouldUpdateStatusAndSuspensionEndDate()
        {
            var before = DateTime.UtcNow;

            var success = await _service.SuspendUserAsync(1, 7);
            var after = DateTime.UtcNow;

            Assert.True(success);

            var user = await _context.Users.FindAsync(1);
            Assert.Equal(UserStatus.Suspended, user.Status);
            Assert.NotNull(user.SuspensionEndDate);
            Assert.InRange(user.SuspensionEndDate.Value, before.AddDays(7), after.AddDays(7));
        }

        [Fact]
        public async Task SuspendUserAsync_NonExistentUser_ShouldReturnFalse()
        {
            var result = await _service.SuspendUserAsync(999, 3);

            Assert.False(result);
        }

        [Fact]
        public async Task BanUserAsync_ShouldSetStatusToBannedAndClearSuspension()
        {
            var user = new User
            {
                Id = 3,
                FirstName = "Charlie",
                LastName = "Doe",
                Email = "charlie@example.com",
                UserName = "charlie@example.com",
                Status = UserStatus.Suspended,
                SuspensionEndDate = DateTime.UtcNow.AddDays(2)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var success = await _service.BanUserAsync(3);

            Assert.True(success);

            var updatedUser = await _context.Users.FindAsync(3);
            Assert.Equal(UserStatus.Banned, updatedUser.Status);
            Assert.Null(updatedUser.SuspensionEndDate);
        }

        [Fact]
        public async Task BanUserAsync_ForHost_ShouldCancelActiveEvents()
        {
            var hostUser = new User
            {
                Id = 4,
                FirstName = "Host",
                LastName = "User",
                Email = "host@example.com",
                UserName = "host@example.com",
                UserType = UserType.Host
            };

            var host = new Host
            {
                Id = 4,
                User = hostUser,
                Events = new List<Event>
                {
                    new Event { Id = 10, Title = "Active Event", Location = "NYC", Price = 150, HostId = 4, Status = EventStatus.Active },
                    new Event { Id = 11, Title = "Canceled Event", Location = "NYC", Price = 90, HostId = 4, Status = EventStatus.Canceled }
                }
            };

            _context.Users.Add(hostUser);
            _context.Events.AddRange(host.Events);
            _context.Hosts.Add(host);
            await _context.SaveChangesAsync();

            var success = await _service.BanUserAsync(4);

            Assert.True(success);

            var events = await _context.Events.Where(e => e.HostId == 4).ToListAsync();
            Assert.All(events, e => Assert.Equal(EventStatus.Canceled, e.Status));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
