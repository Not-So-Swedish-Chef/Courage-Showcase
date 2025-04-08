using back_end.Controllers;
using back_end.Models;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace back_end_tests.Services
{
    public class HostServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly HostService _service;
        private readonly Mock<ILogger<HostService>> _mockLogger;

        public HostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "HostServiceTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<HostService>>();
            _service = new HostService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateHostAsync_CreatesHost_WhenHostDoesNotExist()
        {
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };

            await _service.CreateHostAsync(user);
            var host = await _context.Hosts.FindAsync(user.Id);

            Assert.NotNull(host);
            Assert.Equal(user.Id, host.Id);
        }

        [Fact]
        public async Task CreateHostAsync_DoesNothing_WhenHostExists()
        {
            var user = new User { Id = 2, FirstName = "Test", LastName = "User", Email = "two@example.com" };
            _context.Users.Add(user);
            _context.Hosts.Add(new Host { Id = 2, User = user });
            await _context.SaveChangesAsync();

            await _service.CreateHostAsync(user);
            var count = await _context.Hosts.CountAsync(h => h.Id == 2);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetHostByUserIdAsync_ReturnsNull_WhenHostNotFound()
        {
            var result = await _service.GetHostByUserIdAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetHostByUserIdAsync_ReturnsHost_WhenExists()
        {
            var user = new User { Id = 3, FirstName = "Test", LastName = "User", Email = "three@example.com" };
            _context.Users.Add(user);
            _context.Hosts.Add(new Host { Id = 3, User = user });
            await _context.SaveChangesAsync();

            var result = await _service.GetHostByUserIdAsync(3);

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
        }

        [Fact]
        public async Task UpdateHostInfoAsync_ReturnsFalse_WhenHostNotFound()
        {
            var model = new UpdateHostModel { AgencyName = "New", Bio = "Bio" };

            var result = await _service.UpdateHostInfoAsync(100, model);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateHostInfoAsync_UpdatesHost_WhenExists()
        {
            var user = new User { Id = 4, FirstName = "Test", LastName = "User", Email = "four@example.com" };
            var host = new Host { Id = 4, AgencyName = "Old", Bio = "OldBio", User = user };
            _context.Users.Add(user);
            _context.Hosts.Add(host);
            await _context.SaveChangesAsync();

            var model = new UpdateHostModel { AgencyName = "Updated", Bio = "Updated bio" };
            var result = await _service.UpdateHostInfoAsync(4, model);

            Assert.True(result);
            var updated = await _context.Hosts.FindAsync(4);
            Assert.Equal("Updated", updated.AgencyName);
            Assert.Equal("Updated bio", updated.Bio);
        }
    }
}
