using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using back_end.Models;
using back_end.Models.Api;
using back_end.Services;
using System.Threading.Tasks;

namespace back_end_tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<JwtService>> _mockLogger;
        private readonly JwtService _service;

        public JwtServiceTests()
        {
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<JwtService>>();

            // Setup configuration
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
            _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
            
            // Setup TokenValidityMins using IConfigurationSection
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("60");
            _mockConfiguration.Setup(c => c.GetSection("Jwt:TokenValidityMins")).Returns(mockSection.Object);

            _service = new JwtService(_mockUserManager.Object, _mockSignInManager.Object, 
                _mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ShouldReturnLoginResponse()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "test@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 1, 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User",
                UserType = UserType.Member
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.NotNull(result.SignedInUser);
            Assert.Equal(user.Id, result.SignedInUser.Id);
            Assert.Equal(user.FirstName, result.SignedInUser.FirstName);
            Assert.Equal(user.LastName, result.SignedInUser.LastName);
            Assert.Equal(user.Email, result.SignedInUser.Email);
            Assert.Equal(user.UserType, result.SignedInUser.UserType);
        }

        [Fact]
        public async Task Authenticate_WithEmptyEmail_ShouldReturnNull()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "", Password = "Password123!" };

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithEmptyPassword_ShouldReturnNull()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "test@example.com", Password = "" };

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithNonExistentUser_ShouldReturnNull()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "nonexistent@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "test@example.com", Password = "WrongPassword!" };
            var user = new User 
            { 
                Id = 1, 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User",
                UserType = UserType.Member
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithValidHostCredentials_ShouldReturnTokenWithHostRole()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "host@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 2, 
                Email = "host@example.com", 
                FirstName = "Host", 
                LastName = "User",
                UserType = UserType.Host
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(UserType.Host, result.SignedInUser.UserType);
        }

        [Fact]
        public async Task Authenticate_WithValidAdminCredentials_ShouldReturnTokenWithAdminRole()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "admin@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 3, 
                Email = "admin@example.com", 
                FirstName = "Admin", 
                LastName = "User",
                UserType = UserType.Admin
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(UserType.Admin, result.SignedInUser.UserType);
        }

        [Fact]
        public async Task Authenticate_WhenExceptionOccurs_ShouldReturnNull()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "test@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }
    }
}

