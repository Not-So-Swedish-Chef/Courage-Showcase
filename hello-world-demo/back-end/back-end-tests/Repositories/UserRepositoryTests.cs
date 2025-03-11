using Microsoft.EntityFrameworkCore;
using Xunit;
using back_end.Data;
using back_end.Models;
using back_end.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_tests.Repositories
{
    public class UserRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }


        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist");

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByIdAsync(1);

                // Assert
                Assert.Null(result);
            }
        }

        

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("GetUserByEmailAsync_ReturnsNull_WhenUserDoesNotExist");

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByEmailAsync("nonexistent@example.com");

                // Assert
                Assert.Null(result);
            }
        }

        
    }
}