using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using back_end.Controllers;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Http;

namespace back_end_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IHostService> _mockHostService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockHostService = new Mock<IHostService>();
            _mockUserService = new Mock<IUserService>();

            _controller = new UserController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockConfiguration.Object,
                _mockHostService.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var model = new RegisterModel();

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var model = new LoginModel { Email = "noone@none.com", Password = "badpass" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task SaveEvent_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userId = 1;
            var eventId = 10;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockUserService.Setup(s => s.SaveEventAsync(userId, eventId)).ReturnsAsync(true);

            // Act
            var result = await _controller.SaveEvent(eventId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event saved successfully.", ok.Value);
        }

        [Fact]
        public async Task GetSavedEvents_ReturnsOk_WithEvents()
        {
            // Arrange
            var userId = 1;
            var mockEvents = new List<Event> { new Event { Title = "E1" } };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockUserService.Setup(s => s.GetSavedEventsAsync(userId)).ReturnsAsync(mockEvents);

            // Act
            var result = await _controller.GetSavedEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var events = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            Assert.Single(events);
        }
    }
}