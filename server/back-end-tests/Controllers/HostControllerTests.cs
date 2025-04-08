using AutoMapper;
using back_end.Controllers;
using back_end.DTOs;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace back_end_tests.Controllers
{
    public class HostControllerTests
    {
        private readonly Mock<IHostService> _mockHostService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<HostController>> _mockLogger;
        private readonly HostController _controller;

        public HostControllerTests()
        {
            _mockHostService = new Mock<IHostService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<HostController>>();

            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            _controller = new HostController(
                _mockHostService.Object,
                _mockUserManager.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    }, "mock"))
                }
            };
        }

        [Fact]
        public async Task GetMyEvents_ReturnsOk_WithMappedEvents()
        {
            // Arrange
            var host = new Host
            {
                Id = 1,
                Events = new List<Event> { new Event { Id = 1, Title = "Test Event" } }
            };

            var eventDtos = new List<EventDTO> { new EventDTO { Id = 1, Title = "Test Event" } };

            _mockHostService.Setup(s => s.GetHostByUserIdAsync(1)).ReturnsAsync(host);
            _mockMapper.Setup(m => m.Map<List<EventDTO>>(host.Events)).Returns(eventDtos);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvents = Assert.IsType<List<EventDTO>>(okResult.Value);
            Assert.Single(returnedEvents);
        }

        [Fact]
        public async Task GetMyEvents_ReturnsNotFound_WhenHostIsNull()
        {
            _mockHostService.Setup(s => s.GetHostByUserIdAsync(1)).ReturnsAsync((Host)null);

            var result = await _controller.GetMyEvents();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Host profile not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetMyEvents_ReturnsUnauthorized_WhenUserIdMissing()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetMyEvents();

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User not found.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("AgencyName", "Required");

            var model = new UpdateHostModel { AgencyName = "", Bio = "" };

            var result = await _controller.UpdateHostInfo(model);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsBadRequest_WhenUpdateFails()
        {
            _mockHostService.Setup(s => s.UpdateHostInfoAsync(1, It.IsAny<UpdateHostModel>())).ReturnsAsync(false);

            var result = await _controller.UpdateHostInfo(new UpdateHostModel());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to update host info.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsNoContent_WhenUpdateSucceeds()
        {
            _mockHostService.Setup(s => s.UpdateHostInfoAsync(1, It.IsAny<UpdateHostModel>())).ReturnsAsync(true);

            var result = await _controller.UpdateHostInfo(new UpdateHostModel());

            Assert.IsType<NoContentResult>(result);
        }
    }
}
