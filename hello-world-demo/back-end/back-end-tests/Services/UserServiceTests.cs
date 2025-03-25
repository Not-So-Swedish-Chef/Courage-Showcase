using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace back_end_tests.Services
{
    public class UserServiceTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetSavedEventsAsync_ReturnsSavedEventsForUser()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetSavedEventsAsync_ReturnsSavedEventsForUser");
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@example.com" };
                var event1 = new Event { Id = 1, Title = "Event 1" };
                var event2 = new Event { Id = 2, Title = "Event 2" };
                user.SavedEvents.Add(event1);
                user.SavedEvents.Add(event2);
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UserService(context);

                // Act
                var result = await service.GetSavedEventsAsync(1);

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task SaveEventAsync_AddsEventToUserSavedEvents()
        {
            // Arrange
            var options = GetInMemoryDbOptions("SaveEventAsync_AddsEventToUserSavedEvents");
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
                var evnt = new Event { Id = 1, Title = "Cool Event" };
                context.Users.Add(user);
                context.Events.Add(evnt);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UserService(context);

                // Act
                var result = await service.SaveEventAsync(1, 1);

                // Assert
                Assert.True(result);
                var savedEvents = context.Users.Include(u => u.SavedEvents).First(u => u.Id == 1).SavedEvents;
                Assert.Single(savedEvents);
            }
        }

        [Fact]
        public async Task RemoveSavedEventAsync_RemovesEventFromUserSavedEvents()
        {
            // Arrange
            var options = GetInMemoryDbOptions("RemoveSavedEventAsync_RemovesEventFromUserSavedEvents");
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" };
                var evnt = new Event { Id = 1, Title = "Old Event" };
                user.SavedEvents.Add(evnt);
                context.Users.Add(user);
                context.Events.Add(evnt);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UserService(context);

                // Act
                var result = await service.RemoveSavedEventAsync(1, 1);

                // Assert
                Assert.True(result);
                var savedEvents = context.Users.Include(u => u.SavedEvents).First(u => u.Id == 1).SavedEvents;
                Assert.Empty(savedEvents);
            }
        }
    }
}