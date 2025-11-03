using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using back_end.Controllers;
using back_end.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using back_end.Enums;

namespace back_end_tests.Controllers
{
    public class DisabilityTagControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<DisabilityTagController>> _mockLogger;
        private readonly DisabilityTagController _controller;

        public DisabilityTagControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<DisabilityTagController>>();
            _controller = new DisabilityTagController(_context, _mockLogger.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var tag1 = new DisabilityTag { Id = 1, Name = "Wheelchair Accessible", NormalizedName = "WHEELCHAIR ACCESSIBLE" };
            var tag2 = new DisabilityTag { Id = 2, Name = "ASL Interpreter", NormalizedName = "ASL INTERPRETER" };
            
            var activeEvent = new Event
            {
                Id = 1,
                Title = "Test Event",
                Location = "Test Location",
                City = OntarioCity.Toronto,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Status = EventStatus.Active
            };
            activeEvent.DisabilityTags.Add(tag1);

            _context.DisabilityTags.AddRange(tag1, tag2);
            _context.Events.Add(activeEvent);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllTags_ShouldReturnAllTags()
        {
            // Act
            var result = await _controller.GetAllTags();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tags = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.NotNull(tags);
        }

        [Fact]
        public async Task GetTagById_WithValidId_ShouldReturnTag()
        {
            // Act
            var result = await _controller.GetTagById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tag = Assert.IsType<DisabilityTag>(okResult.Value);
            Assert.Equal(1, tag.Id);
            Assert.Equal("Wheelchair Accessible", tag.Name);
        }

        [Fact]
        public async Task GetTagById_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.GetTagById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateTag_WithValidData_ShouldReturnCreatedTag()
        {
            // Arrange
            var dto = new CreateDisabilityTagDTO { Name = "Audio Description" };

            // Act
            var result = await _controller.CreateTag(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var tag = Assert.IsType<DisabilityTag>(createdResult.Value);
            Assert.Equal("Audio Description", tag.Name);
            Assert.Equal("AUDIO DESCRIPTION", tag.NormalizedName);
        }

        [Fact]
        public async Task CreateTag_WithEmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new CreateDisabilityTagDTO { Name = "" };

            // Act
            var result = await _controller.CreateTag(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Tag name is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTag_WithDuplicateName_ShouldReturnConflict()
        {
            // Arrange
            var dto = new CreateDisabilityTagDTO { Name = "Wheelchair Accessible" };

            // Act
            var result = await _controller.CreateTag(dto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Contains("already exists", conflictResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateTag_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var dto = new UpdateDisabilityTagDTO { Name = "Wheelchair Access" };

            // Act
            var result = await _controller.UpdateTag(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify update
            var tag = await _context.DisabilityTags.FindAsync(1);
            Assert.Equal("Wheelchair Access", tag.Name);
        }

        [Fact]
        public async Task UpdateTag_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var dto = new UpdateDisabilityTagDTO { Name = "Updated Name" };

            // Act
            var result = await _controller.UpdateTag(999, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTag_WithEmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new UpdateDisabilityTagDTO { Name = "" };

            // Act
            var result = await _controller.UpdateTag(1, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tag name is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTag_WithDuplicateName_ShouldReturnConflict()
        {
            // Arrange
            var dto = new UpdateDisabilityTagDTO { Name = "ASL Interpreter" }; // Already exists as tag 2

            // Act
            var result = await _controller.UpdateTag(1, dto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("already exists", conflictResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTag_WithUnusedTag_ShouldReturnNoContent()
        {
            // Arrange - tag2 (ASL Interpreter) is not used by any events

            // Act
            var result = await _controller.DeleteTag(2);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify deletion
            var tag = await _context.DisabilityTags.FindAsync(2);
            Assert.Null(tag);
        }

        [Fact]
        public async Task DeleteTag_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.DeleteTag(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTag_WithTagInUse_ShouldReturnBadRequest()
        {
            // Arrange - tag1 is used by an active event

            // Act
            var result = await _controller.DeleteTag(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("being used by", badRequestResult.Value.ToString());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

