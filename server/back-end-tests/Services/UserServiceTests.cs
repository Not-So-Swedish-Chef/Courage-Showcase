using Moq;
using Xunit;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end_tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User { Email = "user1@example.com" },
                new User { Email = "user2@example.com" }
            };
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(mockUsers);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}