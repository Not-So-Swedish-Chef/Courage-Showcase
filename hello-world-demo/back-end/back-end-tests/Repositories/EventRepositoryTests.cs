using Microsoft.EntityFrameworkCore;
using Xunit;
using back_end.Models;
using back_end.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_tests.Repositories
{
    public class EventRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }


        [Fact]
        public async Task GetEventByIdAsync_ReturnsNull_WhenEventDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions("GetEventByIdAsync_ReturnsNull_WhenEventDoesNotExist");

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new EventRepository(context);

                // Act
                var result = await repository.GetEventByIdAsync(1);

                // Assert
                Assert.Null(result);
            }
        }

        
    }
}