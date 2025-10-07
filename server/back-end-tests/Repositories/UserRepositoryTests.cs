using Microsoft.EntityFrameworkCore;
using Xunit;
using back_end.Models;
using back_end.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var events = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Title = "Tech Conference 2024",
                    Location = "New York",
                    StartDateTime = new DateTime(2024, 6, 15, 9, 0, 0),
                    EndDateTime = new DateTime(2024, 6, 15, 17, 0, 0),
                    Price = 299.99m
                },
                new Event
                {
                    Id = 2,
                    Title = "Music Festival",
                    Location = "Los Angeles",
                    StartDateTime = new DateTime(2024, 7, 20, 18, 0, 0),
                    EndDateTime = new DateTime(2024, 7, 22, 23, 0, 0),
                    Price = 150.00m
                },
                new Event
                {
                    Id = 3,
                    Title = "Art Exhibition",
                    Location = "Chicago",
                    StartDateTime = new DateTime(2024, 5, 10, 10, 0, 0),
                    EndDateTime = new DateTime(2024, 5, 10, 18, 0, 0),
                    Price = 25.50m
                }
            };

            _context.Events.AddRange(events);
            _context.SaveChanges();

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    UserName = "john.doe@example.com",
                    UserType = UserType.Member,
                    SavedEvents = new List<Event> { events[0], events[1] }
                },
                new User
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    UserName = "jane.smith@example.com",
                    UserType = UserType.Host,
                    SavedEvents = new List<Event> { events[2] }
                },
                new User
                {
                    Id = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Email = "bob.johnson@example.com",
                    UserName = "bob.johnson@example.com",
                    UserType = UserType.Admin,
                    SavedEvents = new List<Event>()
                }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsersWithSavedEvents()
        {
            var result = await _repository.GetAllUsersAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());

            var userWithEvents = result.First(u => u.Id == 1);
            Assert.NotNull(userWithEvents.SavedEvents);
            Assert.Equal(2, userWithEvents.SavedEvents.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUserWithSavedEvents()
        {
            int userId = 1;

            var result = await _repository.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.NotNull(result.SavedEvents);
            Assert.Equal(2, result.SavedEvents.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            int invalidId = 999;

            var result = await _repository.GetUserByIdAsync(invalidId);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserSuccessfully()
        {
            var newUser = new User
            {
                FirstName = "Alice",
                LastName = "Wilson",
                Email = "alice.wilson@example.com",
                UserName = "alice.wilson@example.com",
                SavedEvents = new List<Event>()
            };

            await _repository.CreateUserAsync(newUser);

            var allUsers = await _context.Users.ToListAsync();
            Assert.Equal(4, allUsers.Count);

            var addedUser = allUsers.FirstOrDefault(u => u.FirstName == "Alice" && u.LastName == "Wilson");
            Assert.NotNull(addedUser);
            Assert.Equal("alice.wilson@example.com", addedUser.Email);
            Assert.True(addedUser.Id > 0);
        }

        [Fact]
        public async Task CreateUserAsync_WithSavedEvents_ShouldCreateUserWithAssociations()
        {
            var existingEvent = await _context.Events.FindAsync(1);
            var newUser = new User
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "charlie.brown@example.com",
                UserName = "charlie.brown@example.com",
                SavedEvents = new List<Event> { existingEvent }
            };

            await _repository.CreateUserAsync(newUser);

            var createdUser = await _context.Users
                .Include(u => u.SavedEvents)
                .FirstOrDefaultAsync(u => u.FirstName == "Charlie" && u.LastName == "Brown");

            Assert.NotNull(createdUser);
            Assert.Single(createdUser.SavedEvents);
            Assert.Equal("Tech Conference 2024", createdUser.SavedEvents.First().Title);
        }

        [Fact]
        public async Task UpdateUserAsync_WithExistingUser_ShouldUpdateSuccessfully()
        {
            var userToUpdate = await _context.Users.FindAsync(1);
            userToUpdate.FirstName = "John";
            userToUpdate.LastName = "Doe Updated";
            userToUpdate.Email = "john.doe.updated@example.com";

            await _repository.UpdateUserAsync(userToUpdate);

            var updatedUser = await _context.Users.FindAsync(1);
            Assert.NotNull(updatedUser);
            Assert.Equal("Doe Updated", updatedUser.LastName);
            Assert.Equal("john.doe.updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistingUser_ShouldThrowException()
        {
            var nonExistingUser = new User
            {
                Id = 999,
                FirstName = "Non-existing",
                LastName = "User",
                Email = "nonexisting@example.com",
                UserName = "nonexisting@example.com"
            };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await _repository.UpdateUserAsync(nonExistingUser));
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingId_ShouldDeleteUser()
        {
            int userIdToDelete = 3;
            var usersBefore = await _context.Users.CountAsync();

            await _repository.DeleteUserAsync(userIdToDelete);

            var usersAfter = await _context.Users.CountAsync();
            Assert.Equal(usersBefore - 1, usersAfter);

            var deletedUser = await _context.Users.FindAsync(userIdToDelete);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistingId_ShouldNotThrowException()
        {
            int nonExistingId = 999;
            var usersBefore = await _context.Users.CountAsync();

            await _repository.DeleteUserAsync(nonExistingId);

            var usersAfter = await _context.Users.CountAsync();
            Assert.Equal(usersBefore, usersAfter);
        }


        [Fact]
        public async Task GetUserByIdAsync_ShouldIncludeCorrectSavedEvents()
        {
            int userId = 2;

            var result = await _repository.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.Single(result.SavedEvents);
            Assert.Equal("Art Exhibition", result.SavedEvents.First().Title);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserWithNoSavedEvents_ShouldReturnEmptyCollection()
        {
            int userId = 3;

            var result = await _repository.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal("Bob", result.FirstName);
            Assert.Equal("Johnson", result.LastName);
            Assert.NotNull(result.SavedEvents);
            Assert.Empty(result.SavedEvents);
        }

        [Fact]
        public async Task CreateUserAsync_WithNullSavedEvents_ShouldCreateUserSuccessfully()
        {
            var newUser = new User
            {
                FirstName = "David",
                LastName = "Miller",
                Email = "david.miller@example.com",
                UserName = "david.miller@example.com",
                SavedEvents = null
            };

            await _repository.CreateUserAsync(newUser);

            var createdUser = await _context.Users
                .Include(u => u.SavedEvents)
                .FirstOrDefaultAsync(u => u.FirstName == "David" && u.LastName == "Miller");

            Assert.NotNull(createdUser);
            Assert.Equal("david.miller@example.com", createdUser.Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUsersInConsistentOrder()
        {
            var result1 = await _repository.GetAllUsersAsync();
            var result2 = await _repository.GetAllUsersAsync();

            var users1 = result1.OrderBy(u => u.Id).ToList();
            var users2 = result2.OrderBy(u => u.Id).ToList();

            Assert.Equal(users1.Count, users2.Count);
            for (int i = 0; i < users1.Count; i++)
            {
                Assert.Equal(users1[i].Id, users2[i].Id);
                Assert.Equal(users1[i].FirstName, users2[i].FirstName);
                Assert.Equal(users1[i].LastName, users2[i].LastName);
                Assert.Equal(users1[i].Email, users2[i].Email);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
