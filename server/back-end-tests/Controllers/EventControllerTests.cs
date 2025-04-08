using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using back_end.Controllers;
using back_end.Models;
using back_end.DTOs;
using back_end.Services;

namespace back_end_tests.Controllers
{
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly Mock<ILogger<EventController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            _loggerMock = new Mock<ILogger<EventController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new EventController(_loggerMock.Object, _eventServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetEvents_ReturnsOk_WithEventDTOList()
        {
            var events = new List<Event> { new Event { Id = 1 }, new Event { Id = 2 } };
            var eventDtos = new List<EventDTO> { new EventDTO { Id = 1 }, new EventDTO { Id = 2 } };
            _eventServiceMock.Setup(s => s.GetAllEventsAsync()).ReturnsAsync(events);
            _mapperMock.Setup(m => m.Map<List<EventDTO>>(events)).Returns(eventDtos);

            var result = await _controller.GetEvents();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<List<EventDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }

        [Fact]
        public async Task GetEventById_ReturnsNotFound_WhenEventIsNull()
        {
            _eventServiceMock.Setup(s => s.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync((Event)null);

            var result = await _controller.GetEventById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateEvent_ReturnsBadRequest_WhenEventIsNull()
        {
            var result = await _controller.CreateEvent(null);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateEvent_ReturnsUnauthorized_WhenUserIdMismatch()
        {
            var eventItem = new Event { Id = 1, HostId = 2 };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"))
                }
            };

            var result = await _controller.UpdateEvent(1, eventItem);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("You are not authorized to update this event.", unauthorized.Value);
        }

        [Fact]
        public async Task DeleteEvent_ReturnsNotFound_WhenEventNotFound()
        {
            _eventServiceMock.Setup(s => s.GetEventByIdAsync(1)).ReturnsAsync((Event)null);

            var result = await _controller.DeleteEvent(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}