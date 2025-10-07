using back_end.DTOs;
using Xunit;

namespace back_end_tests.DTOs
{
    public class HostDTOTests
    {
        [Fact]
        public void HostDTO_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new HostDTO();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Null(dto.AgencyName);
            Assert.Null(dto.Bio);
            Assert.Null(dto.Email);
            Assert.Null(dto.UserName);
            Assert.Null(dto.Events);
        }

        [Fact]
        public void HostDTO_SetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            var dto = new HostDTO();
            var events = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1" },
                new EventDTO { Id = 2, Title = "Event 2" }
            };

            // Act
            dto.Id = 1;
            dto.AgencyName = "Test Agency";
            dto.Bio = "Test bio about the host";
            dto.Email = "test@example.com";
            dto.UserName = "testuser";
            dto.Events = events;

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Agency", dto.AgencyName);
            Assert.Equal("Test bio about the host", dto.Bio);
            Assert.Equal("test@example.com", dto.Email);
            Assert.Equal("testuser", dto.UserName);
            Assert.Equal(events, dto.Events);
            Assert.Equal(2, dto.Events.Count);
        }

        [Fact]
        public void HostDTO_Events_CanBeNull()
        {
            // Arrange
            var dto = new HostDTO
            {
                Id = 1,
                AgencyName = "Test Agency",
                Events = null
            };

            // Assert
            Assert.Null(dto.Events);
        }

        [Fact]
        public void HostDTO_Events_CanBeEmptyList()
        {
            // Arrange
            var dto = new HostDTO
            {
                Id = 1,
                AgencyName = "Test Agency",
                Events = new List<EventDTO>()
            };

            // Assert
            Assert.NotNull(dto.Events);
            Assert.Empty(dto.Events);
        }

        [Fact]
        public void HostDTO_AgencyName_CanBeNull()
        {
            // Arrange & Act
            var dto = new HostDTO
            {
                Id = 1,
                AgencyName = null
            };

            // Assert
            Assert.Null(dto.AgencyName);
        }

        [Fact]
        public void HostDTO_Bio_CanBeNull()
        {
            // Arrange & Act
            var dto = new HostDTO
            {
                Id = 1,
                Bio = null
            };

            // Assert
            Assert.Null(dto.Bio);
        }

        [Fact]
        public void HostDTO_Email_CanBeNull()
        {
            // Arrange & Act
            var dto = new HostDTO
            {
                Id = 1,
                Email = null
            };

            // Assert
            Assert.Null(dto.Email);
        }

        [Fact]
        public void HostDTO_UserName_CanBeNull()
        {
            // Arrange & Act
            var dto = new HostDTO
            {
                Id = 1,
                UserName = null
            };

            // Assert
            Assert.Null(dto.UserName);
        }

        [Fact]
        public void HostDTO_WithMultipleEvents_PropertiesAreSetCorrectly()
        {
            // Arrange
            var events = new List<EventDTO>
            {
                new EventDTO { Id = 1, Title = "Event 1", Location = "Location 1" },
                new EventDTO { Id = 2, Title = "Event 2", Location = "Location 2" },
                new EventDTO { Id = 3, Title = "Event 3", Location = "Location 3" }
            };

            // Act
            var dto = new HostDTO
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = "Test bio",
                Email = "test@example.com",
                UserName = "testuser",
                Events = events
            };

            // Assert
            Assert.Equal(3, dto.Events.Count);
            Assert.Equal("Event 1", dto.Events[0].Title);
            Assert.Equal("Event 2", dto.Events[1].Title);
            Assert.Equal("Event 3", dto.Events[2].Title);
        }
    }
}


