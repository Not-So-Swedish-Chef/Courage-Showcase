using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using back_end.Controllers;
using back_end.Models;
using back_end.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using back_end.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;

namespace back_end_tests.Controllers
{
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ILogger<EventController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<EventController>>();
            _mockMapper = new Mock<IMapper>();

            _controller = new EventController(_mockLogger.Object, _mockEventService.Object, _mockMapper.Object);
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

        #region GetEvents Tests

        [Fact]
        public async Task GetEvents_ReturnsOkWithEventDtos()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Event 1" },
                new Event { Id = 2, Title = "Event 2" }
            };

            var eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1" },
                new EventDTO { Id = 2, Title = "Event 2" }
            };

            _mockEventService.Setup(x => x.GetAllEventsAsync())
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Equal(2, ((List<EventDTO>)returnValue).Count);
        }

        [Fact]
        public async Task GetEvents_WhenDataExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            _mockEventService.Setup(x => x.GetAllEventsAsync())
                .ThrowsAsync(new DataException("Test exception"));

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving events.", statusCodeResult.Value);
        }

        #endregion

        #region GetEventById Tests

        [Fact]
        public async Task GetEventById_WithValidId_ReturnsOkWithEvent()
        {
            // Arrange
            var eventId = 1;
            var eventItem = new Event { Id = eventId, Title = "Test Event" };

            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync(eventItem);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventId, returnValue.Id);
        }

        [Fact]
        public async Task GetEventById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetEventById_WhenDataExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var eventId = 1;
            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ThrowsAsync(new DataException("Test exception"));

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving the event.", statusCodeResult.Value);
        }

        #endregion

        #region CreateEvent Tests

        //[Fact]
        //public async Task CreateEvent_WithValidEvent_ReturnsCreatedAtAction()
        //{
        //    // Arrange
        //    var eventItem = new Event { Id = 1, Title = "Test Event" };
        //    SetupUserIdentity(1, "test@example.com");

        //    _mockEventService.Setup(x => x.AddEventAsync(eventItem))
        //        .Returns(Task.CompletedTask);

        //    // Act
        //    var result = await _controller.CreateEvent(eventItem);

        //    // Assert
        //    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        //    Assert.Equal(nameof(EventController.GetEventById), createdResult.ActionName);
        //    Assert.Equal(eventItem, createdResult.Value);
        //}

        [Fact]
        public async Task CreateEvent_WithNullEvent_ReturnsBadRequest()
        {
            // Arrange
            SetupUserIdentity(1, "test@example.com");

            // Act
            var result = await _controller.CreateEvent(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Event data is missing.", badRequestResult.Value);
        }

        //[Fact]
        //public async Task CreateEvent_WithMissingUserClaims_ReturnsUnauthorized()
        //{
        //    // Arrange
        //    var eventItem = new Event { Id = 1, Title = "Test Event" };
        //    // Don't set up user identity

        //    // Act
        //    var result = await _controller.CreateEvent(eventItem);

        //    // Assert
        //    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        //    Assert.Equal("User claims missing.", unauthorizedResult.Value);
        //}

        //[Fact]
        //public async Task CreateEvent_WithInvalidModel_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var eventItem = new Event { Id = 1, Title = "" }; // Invalid event
        //    SetupUserIdentity(1, "test@example.com");

        //    _controller.ModelState.AddModelError("Title", "Title is required");

        //    // Act
        //    var result = await _controller.CreateEvent(eventItem);

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result.Result);
        //}

        //[Fact]
        //public async Task CreateEvent_WhenDataExceptionOccurs_ReturnsStatusCode500()
        //{
        //    // Arrange
        //    var eventItem = new Event { Id = 1, Title = "Test Event" };
        //    SetupUserIdentity(1, "test@example.com");

        //    _mockEventService.Setup(x => x.AddEventAsync(eventItem))
        //        .ThrowsAsync(new DataException("Test exception"));

        //    // Act
        //    var result = await _controller.CreateEvent(eventItem);

        //    // Assert
        //    var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        //    Assert.Equal(500, statusCodeResult.StatusCode);
        //    Assert.Equal("An error occurred while creating the event.", statusCodeResult.Value);
        //}

        #endregion

        #region UpdateEvent Tests

        [Fact]
        public async Task UpdateEvent_WithValidEventAndAuthorizedUser_ReturnsNoContent()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var eventItem = new Event { Id = eventId, Title = "Updated Event", HostId = userId };
            SetupUserIdentity(userId, "test@example.com");

            _mockEventService.Setup(x => x.UpdateEventAsync(eventItem, userId.ToString()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateEvent(eventId, eventItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateEvent_WithNullEvent_ReturnsBadRequest()
        {
            // Arrange
            var eventId = 1;
            SetupUserIdentity(1, "test@example.com");

            // Act
            var result = await _controller.UpdateEvent(eventId, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid event data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateEvent_WithZeroId_ReturnsBadRequest()
        {
            // Arrange
            var eventItem = new Event { Id = 1, Title = "Test Event" };
            SetupUserIdentity(1, "test@example.com");

            // Act
            var result = await _controller.UpdateEvent(0, eventItem);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid event data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateEvent_WithUnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var differentHostId = 2;
            var eventItem = new Event { Id = eventId, Title = "Test Event", HostId = differentHostId };
            SetupUserIdentity(userId, "test@example.com");

            // Act
            var result = await _controller.UpdateEvent(eventId, eventItem);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("You are not authorized to update this event.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task UpdateEvent_WhenDataExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var eventItem = new Event { Id = eventId, Title = "Test Event", HostId = userId };
            SetupUserIdentity(userId, "test@example.com");

            _mockEventService.Setup(x => x.UpdateEventAsync(eventItem, userId.ToString()))
                .ThrowsAsync(new DataException("Test exception"));

            // Act
            var result = await _controller.UpdateEvent(eventId, eventItem);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while updating the event.", statusCodeResult.Value);
        }

        #endregion

        #region DeleteEvent Tests

        [Fact]
        public async Task DeleteEvent_WithValidIdAndAuthorizedUser_ReturnsNoContent()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var eventItem = new Event { Id = eventId, Title = "Test Event", HostId = userId };
            SetupUserIdentity(userId, "test@example.com");

            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync(eventItem);

            _mockEventService.Setup(x => x.DeleteEventAsync(eventId, userId.ToString()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_WithNonExistentEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            SetupUserIdentity(1, "test@example.com");

            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_WithUnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var differentHostId = 2;
            var eventItem = new Event { Id = eventId, Title = "Test Event", HostId = differentHostId };
            SetupUserIdentity(userId, "test@example.com");

            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync(eventItem);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("You are not authorized to delete this event.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task DeleteEvent_WhenDataExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            var eventId = 1;
            var userId = 1;
            var eventItem = new Event { Id = eventId, Title = "Test Event", HostId = userId };
            SetupUserIdentity(userId, "test@example.com");

            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync(eventItem);

            _mockEventService.Setup(x => x.DeleteEventAsync(eventId, userId.ToString()))
                .ThrowsAsync(new DataException("Test exception"));

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while deleting the event.", statusCodeResult.Value);
        }

        #endregion

        #region SearchEvents Tests

        [Fact]
        public async Task SearchEvents_WithNoParameters_ReturnsAllEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Event 1" },
                new Event { Id = 2, Title = "Event 2" }
            };

            var eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1" },
                new EventDTO { Id = 2, Title = "Event 2" }
            };

            _mockEventService.Setup(x => x.SearchEventsAsync(null, null, null, null, null))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Equal(2, ((List<EventDTO>)returnValue).Count);
        }

        [Fact]
        public async Task SearchEvents_WithQueryParameter_ReturnsFilteredEvents()
        {
            // Arrange
            var query = "test";
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Test Event" }
            };

            var eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Test Event" }
            };

            _mockEventService.Setup(x => x.SearchEventsAsync(query, null, null, null, null))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(okResult.Value);
            Assert.Single((List<EventDTO>)returnValue);
        }

        [Fact]
        public async Task SearchEvents_WithMinPriceOnly_SetsMaxPriceTo200()
        {
            // Arrange
            var minPrice = 50m;
            var events = new List<Event>();
            var eventDtos = new List<EventDTO>();

            _mockEventService.Setup(x => x.SearchEventsAsync(null, null, null, minPrice, 200m))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents(minPrice: minPrice);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockEventService.Verify(x => x.SearchEventsAsync(null, null, null, minPrice, 200m), Times.Once);
        }

        [Fact]
        public async Task SearchEvents_WithMaxPriceOnly_SetsMinPriceTo0()
        {
            // Arrange
            var maxPrice = 100m;
            var events = new List<Event>();
            var eventDtos = new List<EventDTO>();

            _mockEventService.Setup(x => x.SearchEventsAsync(null, null, null, 0m, maxPrice))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents(maxPrice: maxPrice);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockEventService.Verify(x => x.SearchEventsAsync(null, null, null, 0m, maxPrice), Times.Once);
        }

        [Fact]
        public async Task SearchEvents_WithFromDateOnly_SetsToDateToNull()
        {
            // Arrange
            var fromDate = DateTime.UtcNow.Date;
            var events = new List<Event>();
            var eventDtos = new List<EventDTO>();

            _mockEventService.Setup(x => x.SearchEventsAsync(null, fromDate, null, null, null))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents(from: fromDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockEventService.Verify(x => x.SearchEventsAsync(null, fromDate, null, null, null), Times.Once);
        }

        [Fact]
        public async Task SearchEvents_WithToDateOnly_SetsFromDateToCurrentDate()
        {
            // Arrange
            var toDate = DateTime.UtcNow.Date.AddDays(7);
            var events = new List<Event>();
            var eventDtos = new List<EventDTO>();

            _mockEventService.Setup(x => x.SearchEventsAsync(null, It.IsAny<DateTime>(), toDate, null, null))
                .ReturnsAsync(events);

            _mockMapper.Setup(x => x.Map<List<EventDTO>>(events))
                .Returns(eventDtos);

            // Act
            var result = await _controller.SearchEvents(to: toDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockEventService.Verify(x => x.SearchEventsAsync(null, It.IsAny<DateTime>(), toDate, null, null), Times.Once);
        }

        [Fact]
        public async Task SearchEvents_WhenDataExceptionOccurs_ReturnsStatusCode500()
        {
            // Arrange
            _mockEventService.Setup(x => x.SearchEventsAsync(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>()))
                .ThrowsAsync(new DataException("Test exception"));

            // Act
            var result = await _controller.SearchEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while searching events.", statusCodeResult.Value);
        }

        #endregion
    }
}