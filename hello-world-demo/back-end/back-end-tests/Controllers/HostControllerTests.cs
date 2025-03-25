using Xunit;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using back_end.Controllers;
using back_end.Models;
using back_end.Services;

namespace back_end_tests.Controllers
{
    public class HostControllerTests
    {
        private readonly Mock<IHostService> _mockHostService;
        private readonly HostController _controller;

        public HostControllerTests()
        {
            _mockHostService = new Mock<IHostService>();
            _controller = new HostController(_mockHostService.Object);

            // Fake User.Identity with a user id claim
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetMyEvents_ReturnsOk_WithEventsList()
        {
            // Arrange
            var events = new List<Event> { new Event { Title = "E1" } };
            var host = new Host { Events = events };
            _mockHostService.Setup(s => s.GetHostByUserIdAsync(1)).ReturnsAsync(host);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<Event>>(ok.Value);
            Assert.Single(returnedEvents);
        }

        [Fact]
        public async Task GetMyEvents_ReturnsNotFound_WhenHostNull()
        {
            // Arrange
            _mockHostService.Setup(s => s.GetHostByUserIdAsync(1)).ReturnsAsync((Host)null);

            // Act
            var result = await _controller.GetMyEvents();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("key", "error");

            // Act
            var result = await _controller.UpdateHostInfo(new UpdateHostModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            _mockHostService.Setup(s => s.UpdateHostInfoAsync(1, It.IsAny<UpdateHostModel>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateHostInfo(new UpdateHostModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateHostInfo_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            _mockHostService.Setup(s => s.UpdateHostInfoAsync(1, It.IsAny<UpdateHostModel>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateHostInfo(new UpdateHostModel());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
