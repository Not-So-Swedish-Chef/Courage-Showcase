using back_end.Models;
using Xunit;

namespace back_end_tests.Models
{
    public class HostTests
    {
        [Fact]
        public void Constructor_InitializesCollections()
        {
            // Arrange & Act
            var host = new Host();

            // Assert
            Assert.NotNull(host.Events);
            Assert.Empty(host.Events);
        }

        [Fact]
        public void Constructor_InitializesUser()
        {
            // Arrange & Act
            var host = new Host();

            // Assert
            Assert.NotNull(host.User);
        }

        [Fact]
        public void Host_CanSetAndGetProperties()
        {
            // Arrange
            var host = new Host
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = "Test Bio"
            };

            // Act & Assert
            Assert.Equal(1, host.Id);
            Assert.Equal("Test Agency", host.AgencyName);
            Assert.Equal("Test Bio", host.Bio);
        }
    }
}
