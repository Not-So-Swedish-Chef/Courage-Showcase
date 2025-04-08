using back_end.Models;
using back_end.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace back_end_tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserRepoTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateUserAsync_AddsUser()
        {
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "test@example.com"
            };

            await _repository.CreateUserAsync(user);
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            _context.Users.AddRange(
                new User { FirstName = "One", LastName = "User", Email = "one@example.com", UserName = "one@example.com" },
                new User { FirstName = "Two", LastName = "User", Email = "two@example.com", UserName = "two@example.com" }
            );
            await _context.SaveChangesAsync();

            var users = await _repository.GetAllUsersAsync();

            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsCorrectUser()
        {
            var user = new User { FirstName = "Find", LastName = "Me", Email = "findme@example.com", UserName = "findme@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetUserByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("findme@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUserInfo()
        {
            var user = new User { FirstName = "Update", LastName = "Me", Email = "update@example.com", UserName = "update@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.FirstName = "Updated";
            await _repository.UpdateUserAsync(user);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.Equal("Updated", updatedUser.FirstName);
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUser()
        {
            var user = new User { FirstName = "Delete", LastName = "Me", Email = "delete@example.com", UserName = "delete@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteUserAsync(user.Id);

            var result = await _context.Users.FindAsync(user.Id);
            Assert.Null(result);
        }
    }
}
