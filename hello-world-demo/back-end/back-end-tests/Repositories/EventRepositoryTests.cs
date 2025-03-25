using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using back_end.Models;
using back_end.Repositories;

namespace back_end_tests.Repositories
{
    public class EventRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetEventByIdAsync_ReturnsNull_WhenEventNotFound()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetEventByIdAsync_ReturnsNull");
            using var context = new ApplicationDbContext(options);
            var repository = new EventRepository(context);

            // Act
            var result = await repository.GetEventByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddEventAsync_SavesEventSuccessfully()
        {
            // Arrange
            var options = GetInMemoryDbOptions("AddEventAsync_SavesEventSuccessfully");
            using var context = new ApplicationDbContext(options);
            var repository = new EventRepository(context);
            var newEvent = new Event { Title = "Test Event", HostId = 1 };

            // Act
            await repository.AddEventAsync(newEvent);

            // Assert
            var savedEvent = await context.Events.FirstOrDefaultAsync();
            Assert.NotNull(savedEvent);
            Assert.Equal("Test Event", savedEvent.Title);
        }
    }
}