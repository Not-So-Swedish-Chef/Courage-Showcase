using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using back_end.Controllers;
using back_end.Services;
using System;

namespace back_end_tests.Controllers
{
    public class CloudinaryControllerTests
    {
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly Mock<ILogger<CloudinaryController>> _mockLogger;
        private readonly CloudinaryController _controller;

        public CloudinaryControllerTests()
        {
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _mockLogger = new Mock<ILogger<CloudinaryController>>();
            _controller = new CloudinaryController(_mockCloudinaryService.Object, _mockLogger.Object);
        }

        [Fact]
        public void GenerateUploadSignature_WithDefaultFolder_ShouldReturnOkWithSignature()
        {
            // Arrange
            var expectedParams = new CloudinaryUploadParams
            {
                ApiKey = "test-api-key",
                CloudName = "test-cloud",
                Signature = "test-signature",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Folder = "events",
                UploadUrl = "https://api.cloudinary.com/v1_1/test-cloud/image/upload"
            };

            _mockCloudinaryService.Setup(x => x.GenerateUploadSignature("events"))
                .Returns(expectedParams);

            // Act
            var result = _controller.GenerateUploadSignature();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CloudinaryUploadParams>(okResult.Value);
            Assert.Equal(expectedParams.ApiKey, returnValue.ApiKey);
            Assert.Equal(expectedParams.CloudName, returnValue.CloudName);
            Assert.Equal(expectedParams.Signature, returnValue.Signature);
            Assert.Equal(expectedParams.Folder, returnValue.Folder);
        }

        [Fact]
        public void GenerateUploadSignature_WithCustomFolder_ShouldReturnOkWithSignature()
        {
            // Arrange
            var customFolder = "profiles";
            var expectedParams = new CloudinaryUploadParams
            {
                ApiKey = "test-api-key",
                CloudName = "test-cloud",
                Signature = "test-signature",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Folder = customFolder,
                UploadUrl = "https://api.cloudinary.com/v1_1/test-cloud/image/upload"
            };

            _mockCloudinaryService.Setup(x => x.GenerateUploadSignature(customFolder))
                .Returns(expectedParams);

            // Act
            var result = _controller.GenerateUploadSignature(customFolder);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CloudinaryUploadParams>(okResult.Value);
            Assert.Equal(customFolder, returnValue.Folder);
        }

        [Fact]
        public void GenerateUploadSignature_WhenExceptionOccurs_ShouldReturnStatusCode500()
        {
            // Arrange
            _mockCloudinaryService.Setup(x => x.GenerateUploadSignature(It.IsAny<string>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _controller.GenerateUploadSignature();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorResponse = statusCodeResult.Value;
            Assert.NotNull(errorResponse);
        }

        [Fact]
        public void GenerateUploadSignature_ShouldCallServiceWithCorrectFolder()
        {
            // Arrange
            var folder = "test-folder";
            var expectedParams = new CloudinaryUploadParams
            {
                ApiKey = "test-api-key",
                CloudName = "test-cloud",
                Signature = "test-signature",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Folder = folder,
                UploadUrl = "https://api.cloudinary.com/v1_1/test-cloud/image/upload"
            };

            _mockCloudinaryService.Setup(x => x.GenerateUploadSignature(folder))
                .Returns(expectedParams);

            // Act
            _controller.GenerateUploadSignature(folder);

            // Assert
            _mockCloudinaryService.Verify(x => x.GenerateUploadSignature(folder), Times.Once);
        }
    }
}


