using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using back_end.Controllers;
using back_end.DTOs;
using back_end.Models;
using back_end.Models.Api;
using back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace back_end_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<JwtService> _mockJwtService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHostService> _mockHostService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly ILogger<UserController> _logger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Set up mocks
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserManager = MockUserManager<User>();
            _mockSignInManager = MockSignInManager(_mockUserManager);
            _mockJwtService = new Mock<JwtService>(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockConfiguration.Object,
                new NullLogger<JwtService>());

            _mockMapper = new Mock<IMapper>();
            _mockHostService = new Mock<IHostService>();
            _mockUserService = new Mock<IUserService>();
            _logger = new NullLogger<UserController>();

            // Create the controller with all required dependencies
            _controller = new UserController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockJwtService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockHostService.Object,
                _mockUserService.Object,
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

        // Helper method to create a mock SignInManager
        private Mock<SignInManager<TUser>> MockSignInManager<TUser>(Mock<UserManager<TUser>> userManager) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            return new Mock<SignInManager<TUser>>(
                userManager.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                null,
                null,
                null,
                null);
        }

        // Helper method to set up user identity in the controller
        private void SetupUserIdentity(int userId, string email)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        #region Register Tests

        [Fact]
        public async Task Register_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                UserType = UserType.Member
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Member"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var value = okResult.Value;
            // Use reflection to get the Message property
            var messageProperty = value.GetType().GetProperty("Message");
            Assert.Equal("User registered successfully", messageProperty.GetValue(value));
        }

        [Fact]
        public async Task Register_WithHostUserType_AddsToHostRoleAndCreatesHost()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "host@example.com",
                Password = "Password123!",
                FirstName = "Host",
                LastName = "User",
                UserType = UserType.Host
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Member"))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Host"))
                .ReturnsAsync(IdentityResult.Success);

            _mockHostService.Setup(x => x.CreateHostAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), "Host"), Times.Once);
            _mockHostService.Verify(x => x.CreateHostAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Register_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "invalid-email",
                Password = "short",
                FirstName = "",
                LastName = ""
            };

            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WhenCreateAsyncFails_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" }
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while registering the user.", statusCodeResult.Value);
        }

        #endregion

        #region GetSavedEvents Tests

        [Fact]
        public async Task GetSavedEvents_WhenUserHasSavedEvents_ReturnsOkWithEvents()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            SetupUserIdentity(userId, email);

            var savedEvents = new List<Event>
            {
                new Event { Id = 1, Title = "Event 1" },
                new Event { Id = 2, Title = "Event 2" }
            };

            var eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1" },
                new EventDTO { Id = 2, Title = "Event 2" }
            };

            _mockUserService.Setup(x => x.GetSavedEventsAsync(userId))
                .ReturnsAsync(savedEvents);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(savedEvents))
                .Returns(eventDtos);

            // Act
            var result = await _controller.GetSavedEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Equal(2, ((List<EventDTO>)returnValue).Count);
        }

        [Fact]
        public async Task GetSavedEvents_WhenUserIdNotFound_ReturnsInternalServerError()
        {
            // Arrange - no user identity set up

            // Act
            var result = await _controller.GetSavedEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving saved events.", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetSavedEvents_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.GetSavedEventsAsync(userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetSavedEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving saved events.", statusCodeResult.Value);
        }

        #endregion

        #region SaveEvent Tests

        [Fact]
        public async Task SaveEvent_WhenEventSavedSuccessfully_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.SaveEventAsync(userId, eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SaveEvent(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event saved successfully.", okResult.Value);
        }

        [Fact]
        public async Task SaveEvent_WhenUserIdNotFound_ReturnsInternalServerError()
        {
            // Arrange - no user identity set up
            var eventId = 5;

            // Act
            var result = await _controller.SaveEvent(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while saving the event.", statusCodeResult.Value);
        }

        [Fact]
        public async Task SaveEvent_WhenSaveEventFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.SaveEventAsync(userId, eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SaveEvent(eventId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to save event.", badRequestResult.Value);
        }

        [Fact]
        public async Task SaveEvent_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.SaveEventAsync(userId, eventId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.SaveEvent(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while saving the event.", statusCodeResult.Value);
        }

        #endregion

        #region RemoveSavedEvent Tests

        [Fact]
        public async Task RemoveSavedEvent_WhenEventRemovedSuccessfully_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.RemoveSavedEventAsync(userId, eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveSavedEvent(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event removed successfully.", okResult.Value);
        }

        [Fact]
        public async Task RemoveSavedEvent_WhenUserIdNotFound_ReturnsInternalServerError()
        {
            // Arrange - no user identity set up
            var eventId = 5;

            // Act
            var result = await _controller.RemoveSavedEvent(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while removing the event.", statusCodeResult.Value);
        }

        [Fact]
        public async Task RemoveSavedEvent_WhenRemoveEventFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.RemoveSavedEventAsync(userId, eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveSavedEvent(eventId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to remove event.", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveSavedEvent_WhenExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var userId = 1;
            var email = "test@example.com";
            var eventId = 5;
            SetupUserIdentity(userId, email);

            _mockUserService.Setup(x => x.RemoveSavedEventAsync(userId, eventId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.RemoveSavedEvent(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while removing the event.", statusCodeResult.Value);
        }

        #endregion
    }
}