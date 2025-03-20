using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using back_end.Controllers;
using back_end.Models;
using back_end.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User { Email = "user1@example.com" },
                new User { Email = "user2@example.com" }
            };
            _mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(mockUsers);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnUsers.Count);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtAction_WhenUserIsValid()
        {
            // Arrange
            var newUser = new User { Email = "newuser@example.com" };
            _mockUserService.Setup(service => service.AddUserAsync(newUser)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetUserById", createdAtActionResult.ActionName);
        }
    }
}