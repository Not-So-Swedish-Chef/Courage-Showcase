using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Enums;
using back_end.Models;
using back_end.Models.Api;
using back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace back_end_tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ILogger<JwtService>> _mockLogger;
        private readonly IConfiguration _configuration;

        public JwtServiceTests()
        {
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null,
                null,
                null,
                null);

            _mockLogger = new Mock<ILogger<JwtService>>();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Issuer", "test-issuer"},
                    {"Jwt:Audience", "test-audience"},
                    {"Jwt:Secret", "test-secret-key-that-is-long-enough-for-hmac-sha256"},
                    {"Jwt:TokenValidityMins", "60"}
                })
                .Build();
        }

        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private JwtService CreateService(ApplicationDbContext context)
        {
            return new JwtService(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _configuration,
                context,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ShouldReturnLoginResponse()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);

            var request = new LoginRequestModel { Email = "test@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 1, 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User",
                UserType = UserType.Member
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

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
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "", Password = "Password123!" };

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithEmptyPassword_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "test@example.com", Password = "" };

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithNonExistentUser_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "nonexistent@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
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
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithValidHostCredentials_ShouldReturnTokenWithHostRole()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "host@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 2, 
                Email = "host@example.com", 
                FirstName = "Host", 
                LastName = "User",
                UserType = UserType.Host
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(UserType.Host, result.SignedInUser.UserType);
        }

        [Fact]
        public async Task Authenticate_WithValidAdminCredentials_ShouldReturnTokenWithAdminRole()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "admin@example.com", Password = "Password123!" };
            var user = new User 
            { 
                Id = 3, 
                Email = "admin@example.com", 
                FirstName = "Admin", 
                LastName = "User",
                UserType = UserType.Admin
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(UserType.Admin, result.SignedInUser.UserType);
        }

        [Fact]
        public async Task Authenticate_WithBannedUser_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "banned@example.com", Password = "Password123!" };
            var user = new User
            {
                Id = 4,
                Email = "banned@example.com",
                FirstName = "Banned",
                LastName = "User",
                UserType = UserType.Member,
                Status = UserStatus.Banned
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithSuspendedUserAndActiveSuspension_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "suspended@example.com", Password = "Password123!" };
            var user = new User
            {
                Id = 5,
                Email = "suspended@example.com",
                FirstName = "Suspended",
                LastName = "User",
                UserType = UserType.Member,
                Status = UserStatus.Suspended,
                SuspensionEndDate = DateTime.UtcNow.AddDays(2)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_WithSuspendedUserAndExpiredSuspension_ShouldReactivateAndReturnResponse()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "expired@example.com", Password = "Password123!" };
            var user = new User
            {
                Id = 6,
                Email = "expired@example.com",
                FirstName = "Expired",
                LastName = "User",
                UserType = UserType.Member,
                Status = UserStatus.Suspended,
                SuspensionEndDate = DateTime.UtcNow.AddDays(-1)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UserStatus.Active, user.Status);
            Assert.Null(user.SuspensionEndDate);
        }

        [Fact]
        public async Task Authenticate_WhenExceptionOccurs_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateContext();
            var service = CreateService(context);
            var request = new LoginRequestModel { Email = "test@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await service.Authenticate(request);

            // Assert
            Assert.Null(result);
        }
    }
}

