using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using back_end.Controllers;
using back_end.DTOs;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace back_end_tests.Controllers
{
    public class HostControllerTests
    {
        private readonly Mock<IHostService> _mockHostService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ILogger<HostController> _logger;
        private readonly HostController _controller;

        public HostControllerTests()
        {
            _mockHostService = new Mock<IHostService>();
            _mockUserManager = MockUserManager<User>();
            _mockMapper = new Mock<IMapper>();
            _logger = new NullLogger<HostController>();

            _controller = new HostController(
                _mockHostService.Object,
                _mockUserManager.Object,
                _mockMapper.Object,
                _logger
            );
        }

        // Helper method to create a mock UserManager
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        // Helper method to set up user identity in the controller
        private void SetupUserIdentity(int userId, string email, string role = "Host")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        #region GetMyEvents Tests

        [Fact]
        public async Task GetMyEvents_WithValidUser_ReturnsOkWithEvents()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var host = new Host
            {
                Id = 1,
                Events = new List<Event>
                {
                    new Event { Id = 1, Title = "Event 1" },
                    new Event { Id = 2, Title = "Event 2" }
                }
            };

            var eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1" },
                new EventDTO { Id = 2, Title = "Event 2" }
            };

            _mockHostService.Setup(x => x.GetHostByUserIdAsync(userId))
                .ReturnsAsync(host);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(host.Events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Equal(2, ((List<EventDTO>)returnValue).Count);
        }


        [Fact]
        public async Task GetMyEvents_WhenHostNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            _mockHostService.Setup(x => x.GetHostByUserIdAsync(userId))
                .ReturnsAsync((Host)null);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Host profile not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetMyEvents_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            _mockHostService.Setup(x => x.GetHostByUserIdAsync(userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving the events.", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetMyEvents_WithEmptyEventsList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var host = new Host
            {
                Id = 1,
                Events = new List<Event>()
            };

            var eventDtos = new List<EventDTO>();

            _mockHostService.Setup(x => x.GetHostByUserIdAsync(userId))
                .ReturnsAsync(host);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(host.Events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Empty((List<EventDTO>)returnValue);
        }

        #endregion

        #region UpdateHostInfo Tests

        [Fact]
        public async Task UpdateHostInfo_WithValidModel_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "Updated bio"
            };

            _mockHostService.Setup(x => x.UpdateHostInfoAsync(userId, model))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateHostInfo_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "Updated bio"
            };

            _controller.ModelState.AddModelError("AgencyName", "Agency name is required");

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateHostInfo_WhenUpdateFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "Updated bio"
            };

            _mockHostService.Setup(x => x.UpdateHostInfoAsync(userId, model))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to update host info.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "Updated bio"
            };

            _mockHostService.Setup(x => x.UpdateHostInfoAsync(userId, model))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while updating the host information.", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_WhenUserIdNotFoundInClaims_ReturnsStatusCode500()
        {
            // Arrange - no user identity set up
            var model = new UpdateHostModel
            {
                AgencyName = "New Agency",
                Bio = "Updated bio"
            };

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while updating the host information.", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_WithNullModel_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            // Act
            var result = await _controller.UpdateHostInfo(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateHostInfo_WithEmptyModel_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            var email = "host@example.com";
            SetupUserIdentity(userId, email);

            var model = new UpdateHostModel
            {
                AgencyName = null,
                Bio = null
            };

            _mockHostService.Setup(x => x.UpdateHostInfoAsync(userId, model))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateHostInfo(model);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullHostService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("hostService", () =>
                new HostController(null, _mockUserManager.Object, _mockMapper.Object, _logger));
        }

        [Fact]
        public void Constructor_WithNullUserManager_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("userManager", () =>
                new HostController(_mockHostService.Object, null, _mockMapper.Object, _logger));
        }

        [Fact]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("mapper", () =>
                new HostController(_mockHostService.Object, _mockUserManager.Object, null, _logger));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("logger", () =>
                new HostController(_mockHostService.Object, _mockUserManager.Object, _mockMapper.Object, null));
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var controller = new HostController(_mockHostService.Object, _mockUserManager.Object, _mockMapper.Object, _logger);

            // Assert
            Assert.NotNull(controller);
        }

        #endregion
    }
}