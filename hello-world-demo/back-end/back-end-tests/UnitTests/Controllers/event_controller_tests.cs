//using Xunit;
//using Moq;
//using Microsoft.AspNetCore.Mvc;
//using back_end.Controllers;
//using back_end.Models;
//using back_end.Services;

//namespace back_end_tests.UnitTests.Controllers
//{
//    public class event_controller_tests
//    {
//        [Fact]
//        public void create_event_returns_bad_request_for_invalid_data()
//        {
//            // Arrange
//            var mock_service = new Mock<IEventService>();
//            var controller = new EventController(mock_service.Object);
//            var invalid_event_data = new EventFormData
//            {
//                Title = "", // Invalid: Title is required
//                Description = "Test Description",
//                Location = "Test Location",
//                ImageUrl = "https://example.com/image.jpg",
//                StartDateTime = DateTime.Now,
//                EndDateTime = DateTime.Now.AddHours(2),
//                CategoryId = "1",
//                Price = 10,
//                IsFree = false,
//                Url = "https://example.com"
//            };

//            // Act
//            var result = controller.CreateEvent(invalid_event_data);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public void create_event_returns_ok_for_valid_data()
//        {
//            // Arrange
//            var mock_service = new Mock<IEventService>();
//            var controller = new EventController(mock_service.Object);
//            var valid_event_data = new EventFormData
//            {
//                Title = "Test Event",
//                Description = "Test Description",
//                Location = "Test Location",
//                ImageUrl = "https://example.com/image.jpg",
//                StartDateTime = DateTime.Now,
//                EndDateTime = DateTime.Now.AddHours(2),
//                CategoryId = "1",
//                Price = 10,
//                IsFree = false,
//                Url = "https://example.com"
//            };

//            // Act
//            var result = controller.CreateEvent(valid_event_data);

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//        }
//    }
//}