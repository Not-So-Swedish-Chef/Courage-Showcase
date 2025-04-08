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
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace back_end_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<JwtService> _mockJwtService;
        private readonly Mock<IHostService> _mockHostService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;

        private readonly UserController _controller;

        public UserControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                null, null, null, null);

            _mockJwtService = new Mock<JwtService>(null!, null!, null!, Mock.Of<ILogger<JwtService>>());
            _mockHostService = new Mock<IHostService>();
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockConfig = new Mock<IConfiguration>();

            _controller = new UserController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockJwtService.Object,
                _mockConfig.Object,
                _mockMapper.Object,
                _mockHostService.Object,
                _mockUserService.Object,
                _mockLogger.Object
            );
        }


        [Fact]
        public async Task GetSavedEvents_ReturnsOkWithList()
        {
            // Arrange
            var userId = "1";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockUserService.Setup(s => s.GetSavedEventsAsync(1)).ReturnsAsync(new List<Event>());
            _mockMapper.Setup(m => m.Map<List<EventDTO>>(It.IsAny<List<Event>>()))
                .Returns(new List<EventDTO>());

            // Act
            var result = await _controller.GetSavedEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<List<EventDTO>>(okResult.Value);
        }

        [Fact]
        public async Task SaveEvent_ReturnsOk_WhenSaved()
        {
            // Arrange
            var userId = "2";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            };

            _mockUserService.Setup(s => s.SaveEventAsync(2, 10)).ReturnsAsync(true);

            // Act
            var result = await _controller.SaveEvent(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event saved successfully.", okResult.Value);
        }

        [Fact]
        public async Task RemoveSavedEvent_ReturnsOk_WhenRemoved()
        {
            // Arrange
            var userId = "2";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            };

            _mockUserService.Setup(s => s.RemoveSavedEventAsync(2, 10)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveSavedEvent(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event removed successfully.", okResult.Value);
        }
    }
}
