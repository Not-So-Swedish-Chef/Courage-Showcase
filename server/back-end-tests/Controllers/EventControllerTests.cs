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
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _controller = new EventController(_mockEventService.Object);
        }

        [Fact]
        public async Task GetEvents_ReturnsOkResult_WithListOfEvents()
        {
            // Arrange
            var mockEvents = new List<Event>
            {
                new Event { Title = "Event 1" },
                new Event { Title = "Event 2" }
            };
            _mockEventService.Setup(service => service.GetAllEventsAsync()).ReturnsAsync(mockEvents);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnEvents = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, returnEvents.Count);
        }

        [Fact]
        public async Task GetEventById_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            _mockEventService.Setup(service => service.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.GetEventById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateEvent_ReturnsCreatedAtAction_WhenEventIsValid()
        {
            // Arrange
            var newEvent = new Event { Title = "New Event" };
            _mockEventService.Setup(service => service.AddEventAsync(newEvent)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateEvent(newEvent);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetEventById", createdAtActionResult.ActionName);
        }
    }
}