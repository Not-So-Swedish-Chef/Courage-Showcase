using back_end.Models;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace back_end_tests.Services
{
    public class UserServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_context, loggerMock.Object);
        }

        [Fact]
        public async Task GetSavedEventsAsync_ReturnsEvents_WhenUserExists()
        {
            var event1 = new Event { Id = 1, Title = "A", Location = "X", StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(1) };
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "a@b.com" };
            user.SavedEvents.Add(event1);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetSavedEventsAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task SaveEventAsync_AddsEvent_WhenNotAlreadySaved()
        {
            var user = new User { Id = 2, FirstName = "Alex", LastName = "Smith", Email = "c@d.com" };
            var ev = new Event { Id = 2, Title = "Event", Location = "Room", StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(1) };
            _context.Users.Add(user);
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            var result = await _service.SaveEventAsync(2, 2);

            Assert.True(result);
            var saved = await _context.Users.Include(u => u.SavedEvents).FirstOrDefaultAsync(u => u.Id == 2);
            Assert.Single(saved.SavedEvents);
        }

        [Fact]
        public async Task SaveEventAsync_DoesNothing_WhenAlreadySaved()
        {
            var ev = new Event { Id = 3, Title = "Repeat", Location = "Gym", StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(1) };
            var user = new User { Id = 3, FirstName = "Alex", LastName = "Saved", Email = "save@d.com" };
            user.SavedEvents.Add(ev);
            _context.Users.Add(user);
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            var result = await _service.SaveEventAsync(3, 3);

            Assert.True(result);
            var updated = await _context.Users.Include(u => u.SavedEvents).FirstOrDefaultAsync(u => u.Id == 3);
            Assert.Single(updated.SavedEvents); // still 1
        }

        [Fact]
        public async Task RemoveSavedEventAsync_RemovesEvent_WhenExists()
        {
            var ev = new Event { Id = 4, Title = "Delete", Location = "R", StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(1) };
            var user = new User { Id = 4, FirstName = "Del", LastName = "Test", Email = "r@x.com" };
            user.SavedEvents.Add(ev);
            _context.Users.Add(user);
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            var result = await _service.RemoveSavedEventAsync(4, 4);

            Assert.True(result);
            var updated = await _context.Users.Include(u => u.SavedEvents).FirstOrDefaultAsync(u => u.Id == 4);
            Assert.Empty(updated.SavedEvents);
        }

        [Fact]
        public async Task RemoveSavedEventAsync_ReturnsFalse_WhenUserOrEventNotFound()
        {
            var result = await _service.RemoveSavedEventAsync(99, 99);
            Assert.False(result);
        }
    }
}
