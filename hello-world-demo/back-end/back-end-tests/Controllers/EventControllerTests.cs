using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using back_end.Controllers;
using back_end.Models;
using back_end.Services;

namespace back_end_tests.Controllers
{
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            _controller = new EventController(_eventServiceMock.Object);
        }

        [Fact]
        public async Task GetEvents_ReturnsOkResult_WithListOfEvents()
        {
            var events = new List<Event> { new Event(), new Event() };
            _eventServiceMock.Setup(s => s.GetAllEventsAsync()).ReturnsAsync(events);

            var result = await _controller.GetEvents();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(events, okResult.Value);
        }

        [Fact]
        public async Task GetEventById_ReturnsNotFound_WhenEventIsNull()
        {
            _eventServiceMock.Setup(s => s.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync((Event)null);

            var result = await _controller.GetEventById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateEvent_ReturnsBadRequest_WhenEventIsNull()
        {
            var result = await _controller.CreateEvent(null);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task UpdateEvent_ReturnsNoContent()
        {
            var testEvent = new Event { Id = 1 };

            var result = await _controller.UpdateEvent(1, testEvent);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ReturnsNoContent()
        {
            var result = await _controller.DeleteEvent(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
