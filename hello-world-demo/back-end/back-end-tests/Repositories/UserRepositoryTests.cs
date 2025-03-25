using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using back_end.Models;
using back_end.Repositories;

namespace back_end_tests.Repositories
{
    public class UserRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetUserByIdAsync_ReturnsNull");
            using var context = new ApplicationDbContext(options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_SavesUserSuccessfully()
        {
            // Arrange
            var options = GetInMemoryDbOptions("CreateUserAsync_SavesUserSuccessfully");
            using var context = new ApplicationDbContext(options);
            var repository = new UserRepository(context);
            var newUser = new User { Email = "test@example.com", FirstName = "Test", LastName = "User", UserName = "test@example.com" };

            // Act
            await repository.CreateUserAsync(newUser);

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync();
            Assert.NotNull(savedUser);
            Assert.Equal("test@example.com", savedUser.Email);
        }
    }
}