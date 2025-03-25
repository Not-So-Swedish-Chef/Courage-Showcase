using System.Threading.Tasks;
using back_end.Models;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using back_end.Controllers;

namespace back_end_tests.Services
{
    public class HostServiceTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }


        [Fact]
        public async Task CreateHostAsync_AddsNewHost_WhenNotExists()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("CreateHostDb");
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            using (var context = new ApplicationDbContext(options))
            {
                var service = new HostService(context);

                // Act
                await service.CreateHostAsync(user);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var host = await context.Hosts.FindAsync(1);
                Assert.NotNull(host);
                Assert.Equal(user.Id, host.Id);
            }
        }

        [Fact]
        public async Task GetHostByUserIdAsync_ReturnsHostWithEvents()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("GetHostDb");

            using (var context = new ApplicationDbContext(options))
            {
                context.Hosts.Add(new Host
                {
                    Id = 1,
                    AgencyName = "Agency",
                    Bio = "Bio here",
                    User = new User { Id = 1, FirstName = "Anna" },
                    Events = new List<Event> { new Event { Title = "Event 1", HostId = 1 } }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new HostService(context);

                // Act
                var result = await service.GetHostByUserIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.Events);
            }
        }

        [Fact]
        public async Task UpdateHostInfoAsync_UpdatesHostData()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("UpdateHostInfoAsync_UpdatesHostData");

            using (var context = new ApplicationDbContext(options))
            {
                var user = new User
                {
                    Id = 1,
                    FirstName = "Evan",
                    LastName = "Doe",
                    Email = "evan@example.com",
                    UserName = "evan@example.com"
                };

                var host = new Host
                {
                    Id = 1,
                    AgencyName = "Old Agency",
                    Bio = "Old Bio",
                    User = user
                };

                context.Users.Add(user);
                context.Hosts.Add(host);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new HostService(context);
                var model = new UpdateHostModel
                {
                    AgencyName = "New Agency",
                    Bio = "New Bio"
                };

                // Act
                var result = await service.UpdateHostInfoAsync(1, model);

                // Assert
                Assert.True(result);
                var updatedHost = await context.Hosts.FirstOrDefaultAsync(h => h.Id == 1);
                Assert.Equal("New Agency", updatedHost.AgencyName);
                Assert.Equal("New Bio", updatedHost.Bio);
            }
        }


        [Fact]
        public async Task UpdateHostInfoAsync_ReturnsFalse_WhenHostNotFound()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("NonexistentHostDb");
            using var context = new ApplicationDbContext(options);
            var service = new HostService(context);

            var model = new UpdateHostModel { AgencyName = "None", Bio = "None" };

            // Act
            var result = await service.UpdateHostInfoAsync(99, model);

            // Assert
            Assert.False(result);
        }
    }
}