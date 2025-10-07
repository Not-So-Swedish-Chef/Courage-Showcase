using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using back_end.Services;
using System;
using System.Collections.Generic;

namespace back_end_tests.Services
{
    public class CloudinaryServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<CloudinaryService>> _mockLogger;
        private readonly CloudinaryService _service;

        public CloudinaryServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<CloudinaryService>>();

            // Setup configuration mock
            _mockConfiguration.Setup(c => c["Cloudinary:CloudName"]).Returns("test-cloud");
            _mockConfiguration.Setup(c => c["Cloudinary:ApiKey"]).Returns("test-api-key");
            _mockConfiguration.Setup(c => c["Cloudinary:ApiSecret"]).Returns("test-api-secret");

            _service = new CloudinaryService(_mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithMissingCloudName_ShouldThrowException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Cloudinary:CloudName"]).Returns((string)null);
            mockConfig.Setup(c => c["Cloudinary:ApiKey"]).Returns("test-api-key");
            mockConfig.Setup(c => c["Cloudinary:ApiSecret"]).Returns("test-api-secret");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new CloudinaryService(mockConfig.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithMissingApiKey_ShouldThrowException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Cloudinary:CloudName"]).Returns("test-cloud");
            mockConfig.Setup(c => c["Cloudinary:ApiKey"]).Returns((string)null);
            mockConfig.Setup(c => c["Cloudinary:ApiSecret"]).Returns("test-api-secret");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new CloudinaryService(mockConfig.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithMissingApiSecret_ShouldThrowException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Cloudinary:CloudName"]).Returns("test-cloud");
            mockConfig.Setup(c => c["Cloudinary:ApiKey"]).Returns("test-api-key");
            mockConfig.Setup(c => c["Cloudinary:ApiSecret"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new CloudinaryService(mockConfig.Object, _mockLogger.Object));
        }

        [Fact]
        public void GenerateUploadSignature_WithDefaultFolder_ShouldReturnValidParams()
        {
            // Act
            var result = _service.GenerateUploadSignature();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-api-key", result.ApiKey);
            Assert.Equal("test-cloud", result.CloudName);
            Assert.Equal("events", result.Folder);
            Assert.NotNull(result.Signature);
            Assert.NotEmpty(result.Signature);
            Assert.True(result.Timestamp > 0);
            Assert.Equal("https://api.cloudinary.com/v1_1/test-cloud/image/upload", result.UploadUrl);
        }

        [Fact]
        public void GenerateUploadSignature_WithCustomFolder_ShouldReturnParamsWithCustomFolder()
        {
            // Arrange
            var customFolder = "profiles";

            // Act
            var result = _service.GenerateUploadSignature(customFolder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customFolder, result.Folder);
            Assert.NotNull(result.Signature);
            Assert.NotEmpty(result.Signature);
        }

        [Fact]
        public void GenerateUploadSignature_ShouldGenerateDifferentSignaturesForDifferentFolders()
        {
            // Arrange
            var folder1 = "events";
            var folder2 = "profiles";

            // Act
            var result1 = _service.GenerateUploadSignature(folder1);
            var result2 = _service.GenerateUploadSignature(folder2);

            // Assert
            Assert.NotEqual(result1.Signature, result2.Signature);
        }

        [Fact]
        public void GenerateUploadSignature_ShouldIncludeTimestamp()
        {
            // Arrange
            var beforeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Act
            var result = _service.GenerateUploadSignature();
            
            var afterTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Assert
            Assert.InRange(result.Timestamp, beforeTimestamp, afterTimestamp);
        }

        [Fact]
        public void GenerateUploadSignature_ShouldGenerateValidSha256Signature()
        {
            // Act
            var result = _service.GenerateUploadSignature();

            // Assert
            // SHA256 hash should be 64 characters (256 bits = 32 bytes = 64 hex characters)
            Assert.Equal(64, result.Signature.Length);
            // Should only contain hexadecimal characters
            Assert.Matches("^[a-f0-9]+$", result.Signature);
        }

        [Fact]
        public void GenerateUploadSignature_ShouldSetCorrectUploadUrl()
        {
            // Act
            var result = _service.GenerateUploadSignature();

            // Assert
            Assert.Contains("test-cloud", result.UploadUrl);
            Assert.Contains("cloudinary.com", result.UploadUrl);
            Assert.Contains("/image/upload", result.UploadUrl);
        }

        [Fact]
        public void GenerateUploadSignature_MultipleCalls_ShouldGenerateDifferentSignatures()
        {
            // Act
            var result1 = _service.GenerateUploadSignature("events");
            System.Threading.Thread.Sleep(1000); // Wait 1 second to ensure different timestamp
            var result2 = _service.GenerateUploadSignature("events");

            // Assert
            // Different timestamps should result in different signatures
            Assert.NotEqual(result1.Timestamp, result2.Timestamp);
            Assert.NotEqual(result1.Signature, result2.Signature);
        }
    }
}


