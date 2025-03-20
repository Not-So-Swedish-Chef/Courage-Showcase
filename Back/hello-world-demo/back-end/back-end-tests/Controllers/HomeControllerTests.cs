//using Microsoft.AspNetCore.Mvc;
//using Xunit;
//using back_end.Controllers;

//namespace back_end_tests.Controllers
//{
//    public class HomeControllerTests
//    {
//        [Fact]
//        public void Get_ReturnsOkResult_WithMessage()
//        {
//            // Arrange
//            var controller = new HomeController();

//            // Act
//            var result = controller.Get();

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            dynamic response = okResult.Value;
//            Assert.Equal("Hello from API", response.Message);
//        }
//    }
//}