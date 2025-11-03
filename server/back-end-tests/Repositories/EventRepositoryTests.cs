using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using back_end.Models;
using back_end.Repositories;
using back_end.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_tests.Repositories
{
    public class EventRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<EventRepository>> _mockLogger;
        private readonly EventRepository _repository;

        public EventRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<EventRepository>>();
            _repository = new EventRepository(_context, _mockLogger.Object);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create a test host user
            var testUser = new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Host",
                Email = "testhost@example.com",
                UserName = "testhost@example.com",
                UserType = UserType.Host
            };

            var testHost = new Host
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = "Test bio",
                User = testUser
            };

            _context.Users.Add(testUser);
            _context.Hosts.Add(testHost);
            _context.SaveChanges();

            var events = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Title = "Tech Conference 2024",
                    Location = "New York",
                    City = OntarioCity.Toronto,
                    StartDateTime = new DateTime(2024, 6, 15, 9, 0, 0),
                    EndDateTime = new DateTime(2024, 6, 15, 17, 0, 0),
                    Price = 299.99m,
                    Status = EventStatus.Active,
                    MinAge = 18,
                    MaxAge = 65,
                    HostId = 1
                },
                new Event
                {
                    Id = 2,
                    Title = "Music Festival",
                    Location = "Los Angeles",
                    City = OntarioCity.Ottawa,
                    StartDateTime = new DateTime(2024, 7, 20, 18, 0, 0),
                    EndDateTime = new DateTime(2024, 7, 22, 23, 0, 0),
                    Price = 150.00m,
                    Status = EventStatus.Active,
                    MinAge = 16,
                    HostId = 1
                },
                new Event
                {
                    Id = 3,
                    Title = "Art Exhibition",
                    Location = "Chicago",
                    City = OntarioCity.Hamilton,
                    StartDateTime = new DateTime(2024, 5, 10, 10, 0, 0),
                    EndDateTime = new DateTime(2024, 5, 10, 18, 0, 0),
                    Price = 25.50m,
                    Status = EventStatus.Active,
                    HostId = 1
                }
            };

            _context.Events.AddRange(events);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Act
            var result = await _repository.GetAllEventsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
        {
            // Arrange
            int eventId = 1;

            // Act
            var result = await _repository.GetEventByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            Assert.Equal("Tech Conference 2024", result.Title);
        }

        [Fact]
        public async Task GetEventByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int invalidId = 999;

            // Act
            var result = await _repository.GetEventByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEventByIdIncludingCanceledAsync_WithValidId_ShouldReturnEvent()
        {
            // Arrange
            int eventId = 1;

            // Act
            var result = await _repository.GetEventByIdIncludingCanceledAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            Assert.Equal("Tech Conference 2024", result.Title);
        }

        [Fact]
        public async Task GetEventByIdIncludingCanceledAsync_WithCanceledEvent_ShouldReturnEvent()
        {
            // Arrange
            var canceledEvent = new Event
            {
                Id = 99,
                Title = "Canceled Event",
                Location = "Test Location",
                City = OntarioCity.Toronto,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Status = EventStatus.Canceled,
                HostId = 1
            };
            _context.Events.Add(canceledEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetEventByIdIncludingCanceledAsync(99);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.Id);
            Assert.Equal(EventStatus.Canceled, result.Status);
        }

        [Fact]
        public async Task AddEventAsync_ShouldAddEventSuccessfully()
        {
            // Arrange
            var newEvent = new Event
            {
                Title = "New Workshop",
                Location = "Boston",
                City = OntarioCity.Toronto,
                StartDateTime = new DateTime(2024, 8, 1, 14, 0, 0),
                EndDateTime = new DateTime(2024, 8, 1, 16, 0, 0),
                Price = 75.00m,
                Status = EventStatus.Active,
                HostId = 1
            };

            // Act
            await _repository.AddEventAsync(newEvent);

            // Assert
            var allEvents = await _context.Events.ToListAsync();
            Assert.Equal(4, allEvents.Count);

            var addedEvent = allEvents.FirstOrDefault(e => e.Title == "New Workshop");
            Assert.NotNull(addedEvent);
            Assert.Equal("Boston", addedEvent.Location);
        }

        [Fact]
        public async Task UpdateEventAsync_WithExistingEvent_ShouldUpdateSuccessfully()
        {
            // Arrange
            var eventToUpdate = await _context.Events.FindAsync(1);
            eventToUpdate.Title = "Updated Tech Conference 2024";
            eventToUpdate.Price = 349.99m;

            // Act
            await _repository.UpdateEventAsync(eventToUpdate);

            // Assert
            var updatedEvent = await _context.Events.FindAsync(1);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Updated Tech Conference 2024", updatedEvent.Title);
            Assert.Equal(349.99m, updatedEvent.Price);
        }

        [Fact]
        public async Task UpdateEventAsync_WithNonExistingEvent_ShouldNotThrowException()
        {
            // Arrange
            var nonExistingEvent = new Event
            {
                Id = 999,
                Title = "Non-existing Event",
                Location = "Nowhere",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(2),
                Price = 100.00m
            };

            // Act & Assert
            await _repository.UpdateEventAsync(nonExistingEvent);
            // Should complete without throwing an exception
        }

        [Fact]
        public async Task DeleteEventAsync_WithNonExistingId_ShouldNotThrowException()
        {
            // Arrange
            int nonExistingId = 999;
            var eventsBefore = await _context.Events.CountAsync();

            // Act
            await _repository.DeleteEventAsync(nonExistingId);

            // Assert
            var eventsAfter = await _context.Events.CountAsync();
            Assert.Equal(eventsBefore, eventsAfter);
            // Should complete without throwing an exception
        }

        [Fact]
        public async Task SearchEventsAsync_WithoutFilters_ShouldReturnAllEvents()
        {
            // Act
            var result = await _repository.SearchEventsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchEventsAsync_WithTitleQuery_ShouldReturnMatchingEvents()
        {
            // Act
            var result = await _repository.SearchEventsAsync(query: "Tech");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("Tech", result.First().Title);
        }

        [Fact]
        public async Task SearchEventsAsync_WithLocationQuery_ShouldReturnMatchingEvents()
        {
            // Act
            var result = await _repository.SearchEventsAsync(query: "Angeles");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("Angeles", result.First().Location);
        }

        [Fact]
        public async Task SearchEventsAsync_WithDateRange_ShouldReturnEventsInRange()
        {
            // Arrange
            var fromDate = new DateTime(2024, 6, 1);
            var toDate = new DateTime(2024, 7, 31);

            // Act
            var result = await _repository.SearchEventsAsync(from: fromDate, to: toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, e =>
            {
                Assert.True(e.StartDateTime >= fromDate);
                Assert.True(e.StartDateTime <= toDate);
            });
        }

        [Fact]
        public async Task SearchEventsAsync_WithMinPrice_ShouldReturnEventsAboveMinPrice()
        {
            // Act
            var result = await _repository.SearchEventsAsync(minPrice: 100m);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, e => Assert.True(e.Price >= 100m));
        }

        [Fact]
        public async Task SearchEventsAsync_WithMaxPrice_ShouldReturnEventsBelowMaxPrice()
        {
            // Act
            var result = await _repository.SearchEventsAsync(maxPrice: 200m);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, e => Assert.True(e.Price <= 200m));
        }

        [Fact]
        public async Task SearchEventsAsync_WithMultipleFilters_ShouldReturnFilteredResults()
        {
            // Act
            var result = await _repository.SearchEventsAsync(
                query: "Festival",
                from: new DateTime(2024, 7, 1),
                to: new DateTime(2024, 7, 31),
                minPrice: 100m,
                maxPrice: 200m);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var resultEvent = result.First();
            Assert.Contains("Festival", resultEvent.Title);
            Assert.True(resultEvent.StartDateTime >= new DateTime(2024, 7, 1));
            Assert.True(resultEvent.StartDateTime <= new DateTime(2024, 7, 31));
            Assert.True(resultEvent.Price >= 100m);
            Assert.True(resultEvent.Price <= 200m);
        }

        [Fact]
        public async Task SearchEventsAsync_WithNoMatches_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.SearchEventsAsync(query: "NonExistentEvent");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchEventsAsync_WithEmptyQuery_ShouldIgnoreQueryFilter()
        {
            // Act
            var result = await _repository.SearchEventsAsync(query: "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchEventsAsync_WithWhitespaceQuery_ShouldIgnoreQueryFilter()
        {
            // Act
            var result = await _repository.SearchEventsAsync(query: "   ");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchEventsAsync_WithCities_ShouldReturnMatchingEvents()
        {
            // Arrange
            var cities = new List<string> { "Toronto", "Ottawa" };

            // Act
            var result = await _repository.SearchEventsAsync(cities: cities);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, e => Assert.Contains(e.City.ToString(), cities, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchEventsAsync_WithSingleCity_ShouldReturnMatchingEvents()
        {
            // Arrange
            var cities = new List<string> { "Toronto" };

            // Act
            var result = await _repository.SearchEventsAsync(cities: cities);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(OntarioCity.Toronto, result.First().City);
        }

        [Fact]
        public async Task SearchEventsAsync_WithAge_ShouldReturnAgeAppropriateEvents()
        {
            // Arrange
            var age = 20;

            // Act
            var result = await _repository.SearchEventsAsync(age: age);

            // Assert
            Assert.NotNull(result);
            // Should return events where age is within MinAge/MaxAge range or where age restrictions don't exist
            Assert.All(result, e =>
            {
                if (e.MinAge.HasValue && e.MaxAge.HasValue)
                {
                    Assert.True(age >= e.MinAge.Value && age <= e.MaxAge.Value);
                }
            });
        }

        [Fact]
        public async Task SearchEventsAsync_WithAgeBelowMinAge_ShouldNotReturnEvent()
        {
            // Arrange
            var age = 15; // Below minAge of 18 for event 1 and 16 for event 2

            // Act
            var result = await _repository.SearchEventsAsync(age: age);

            // Assert
            Assert.NotNull(result);
            // Should only return events with no age restrictions
            Assert.DoesNotContain(result, e => e.MinAge.HasValue && age < e.MinAge.Value);
        }

        [Fact]
        public async Task SearchEventsAsync_WithAgeAboveMaxAge_ShouldNotReturnEvent()
        {
            // Arrange
            var age = 70; // Above maxAge of 65 for event 1

            // Act
            var result = await _repository.SearchEventsAsync(age: age);

            // Assert
            Assert.NotNull(result);
            // Should not include events where age exceeds MaxAge
            Assert.DoesNotContain(result, e => e.MaxAge.HasValue && age > e.MaxAge.Value);
        }

        [Fact]
        public async Task SearchEventsAsync_WithAllFilters_ShouldReturnFilteredResults()
        {
            // Arrange
            var query = "Tech";
            var from = new DateTime(2024, 6, 1);
            var to = new DateTime(2024, 6, 30);
            var minPrice = 200m;
            var maxPrice = 300m;
            var cities = new List<string> { "Toronto" };
            var age = 25;

            // Act
            var result = await _repository.SearchEventsAsync(query, from, to, minPrice, maxPrice, null, cities, age);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var resultEvent = result.First();
            Assert.Contains("Tech", resultEvent.Title);
            Assert.True(resultEvent.StartDateTime >= from);
            Assert.True(resultEvent.StartDateTime <= to);
            Assert.True(resultEvent.Price >= minPrice);
            Assert.True(resultEvent.Price <= maxPrice);
            Assert.Equal(OntarioCity.Toronto, resultEvent.City);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}