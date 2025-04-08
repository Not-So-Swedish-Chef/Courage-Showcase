using back_end.Models.Api;
using back_end.Models;
using Xunit;

namespace back_end_tests.DTOs
{
    public class LoginResponseModelTests
    {
        [Fact]
        public void LoginResponseModel_CanStoreTokenAndUser()
        {
            // Arrange
            var response = new LoginResponseModel
            {
                Token = "mock-jwt-token",
                SignedInUser = new UserRes
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    UserType = UserType.Host
                }
            };

            // Assert
            Assert.Equal("mock-jwt-token", response.Token);
            Assert.NotNull(response.SignedInUser);
            Assert.Equal(1, response.SignedInUser.Id);
            Assert.Equal("John", response.SignedInUser.FirstName);
            Assert.Equal("Doe", response.SignedInUser.LastName);
            Assert.Equal("john@example.com", response.SignedInUser.Email);
            Assert.Equal(UserType.Host, response.SignedInUser.UserType);
        }
    }
}
