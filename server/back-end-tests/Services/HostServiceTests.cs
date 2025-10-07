using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using back_end.Models;
using back_end.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using Host = back_end.Models.Host;
using back_end.Controllers;

namespace back_end_tests.Services
{
    public class HostServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HostService _service;
        private readonly Mock<ILogger<HostService>> _mockLogger;

        public HostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<HostService>>();
            _service = new HostService(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var user1 = new User { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", UserName = "alice@example.com" };
            var user2 = new User { Id = 2, FirstName = "Bob", LastName = "Jones", Email = "bob@example.com", UserName = "bob@example.com" };

            var host1 = new Host
            {
                Id = 1,
                User = user1,
                AgencyName = "Agency A",
                Bio = "Host Alice Bio"
            };

            var host2 = new Host
            {
                Id = 2,
                User = user2,
                AgencyName = "Agency B",
                Bio = "Host Bob Bio"
            };

            _context.Users.AddRange(user1, user2);
            _context.Hosts.AddRange(host1, host2);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateHostAsync_ShouldCreateNewHost_WhenNotExists()
        {
            var user = new User { Id = 3, FirstName = "Charlie", LastName = "Doe", Email = "charlie@example.com", UserName = "charlie@example.com" };

            await _service.CreateHostAsync(user);

            var host = await _context.Hosts.FindAsync(3);
            Assert.NotNull(host);
            Assert.Equal(user.Id, host.Id);
        }

        [Fact]
        public async Task CreateHostAsync_ShouldNotDuplicate_WhenHostAlreadyExists()
        {
            var user = await _context.Users.FindAsync(1);

            await _service.CreateHostAsync(user);

            var hosts = _context.Hosts.Where(h => h.Id == 1).ToList();
            Assert.Single(hosts);
        }

        [Fact]
        public async Task GetHostByUserIdAsync_ShouldReturnHost_WhenExists()
        {
            var host = await _service.GetHostByUserIdAsync(1);

            Assert.NotNull(host);
            Assert.Equal("Agency A", host.AgencyName);
        }

        [Fact]
        public async Task GetHostByUserIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var host = await _service.GetHostByUserIdAsync(999);

            Assert.Null(host);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No host found for user ID 999")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateHostInfoAsync_ShouldUpdateHost_WhenExists()
        {
            var model = new UpdateHostModel
            {
                AgencyName = "Updated Agency",
                Bio = "Updated Bio"
            };

            var success = await _service.UpdateHostInfoAsync(1, model);

            Assert.True(success);

            var host = await _context.Hosts.FindAsync(1);
            Assert.Equal("Updated Agency", host.AgencyName);
            Assert.Equal("Updated Bio", host.Bio);
        }

        [Fact]
        public async Task UpdateHostInfoAsync_ShouldReturnFalse_WhenHostNotFound()
        {
            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "New Bio"
            };

            var success = await _service.UpdateHostInfoAsync(999, model);

            Assert.False(success);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Attempted to update non-existent host for user ID 999")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
