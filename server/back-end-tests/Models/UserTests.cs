using back_end.Models;
using Xunit;

namespace back_end_tests.Models
{
    public class UserTests
    {
        [Fact]
        public void Constructor_InitializesSavedEvents()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.NotNull(user.SavedEvents);
            Assert.Empty(user.SavedEvents);
        }

        [Fact]
        public void Constructor_SetsDefaultUserTypeToMember()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal(UserType.Member, user.UserType);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Ahmed",
                LastName = "Muqarrib",
                Email = "test@example.com",
                UserName = "testuser",
                UserType = UserType.Host
            };

            // Act & Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("Ahmed", user.FirstName);
            Assert.Equal("Muqarrib", user.LastName);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("testuser", user.UserName);
            Assert.Equal(UserType.Host, user.UserType);
        }
    }
}
