using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using back_end.Models.Api;
using back_end.Models;

namespace back_end.Tests.Models.Api
{
    public class LoginResponseModelTests
    {
        [Fact]
        public void LoginResponseModel_DefaultConstructor_InitializesWithNullValues()
        {
            // Act
            var model = new LoginResponseModel();

            // Assert
            Assert.Null(model.Token);
            Assert.Null(model.SignedInUser);
        }

        [Fact]
        public void LoginResponseModel_SetProperties_SetsCorrectly()
        {
            // Arrange
            var userRes = new UserRes
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserType = UserType.Member
            };

            // Act
            var model = new LoginResponseModel
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                SignedInUser = userRes
            };

            // Assert
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", model.Token);
            Assert.Equal(userRes, model.SignedInUser);
        }

        [Fact]
        public void LoginResponseModel_TokenCanBeNull_NoValidationErrors()
        {
            // Arrange
            var model = new LoginResponseModel
            {
                Token = null,
                SignedInUser = new UserRes()
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void LoginResponseModel_SignedInUserCanBeNull_NoValidationErrors()
        {
            // Arrange
            var model = new LoginResponseModel
            {
                Token = "valid.jwt.token",
                SignedInUser = null
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("simple-token")]
        [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
        public void LoginResponseModel_VariousTokenFormats_AcceptedWithoutValidation(string token)
        {
            // Arrange & Act
            var model = new LoginResponseModel
            {
                Token = token
            };

            // Assert
            Assert.Equal(token, model.Token);

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Fact]
        public void LoginResponseModel_CompleteResponse_SetsAllProperties()
        {
            // Arrange
            var userRes = new UserRes
            {
                Id = 123,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@company.com",
                UserType = UserType.Host
            };

            // Act
            var model = new LoginResponseModel
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.payload.signature",
                SignedInUser = userRes
            };

            // Assert
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.payload.signature", model.Token);
            Assert.NotNull(model.SignedInUser);
            Assert.Equal(123, model.SignedInUser.Id);
            Assert.Equal("Jane", model.SignedInUser.FirstName);
            Assert.Equal("Smith", model.SignedInUser.LastName);
            Assert.Equal("jane.smith@company.com", model.SignedInUser.Email);
            Assert.Equal(UserType.Host, model.SignedInUser.UserType);
        }
    }

    public class UserResTests
    {
        [Fact]
        public void UserRes_DefaultConstructor_InitializesWithDefaults()
        {
            // Act
            var userRes = new UserRes();

            // Assert
            Assert.Equal(0, userRes.Id);
            Assert.Null(userRes.FirstName);
            Assert.Null(userRes.LastName);
            Assert.Null(userRes.Email);
            Assert.Equal(UserType.Admin, userRes.UserType); // Default enum value
        }

        [Fact]
        public void UserRes_SetProperties_SetsCorrectly()
        {
            // Act
            var userRes = new UserRes
            {
                Id = 42,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.org",
                UserType = UserType.Admin
            };

            // Assert
            Assert.Equal(42, userRes.Id);
            Assert.Equal("Alice", userRes.FirstName);
            Assert.Equal("Johnson", userRes.LastName);
            Assert.Equal("alice.johnson@example.org", userRes.Email);
            Assert.Equal(UserType.Admin, userRes.UserType);
        }

        [Theory]
        [InlineData(UserType.Admin)]
        [InlineData(UserType.Host)]
        [InlineData(UserType.Member)]
        public void UserRes_UserType_CanBeSetToAnyValidType(UserType userType)
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                UserType = userType
            };

            // Assert
            Assert.Equal(userType, userRes.UserType);
        }

        [Fact]
        public void UserRes_AllPropertiesCanBeNull_NoValidationErrors()
        {
            // Arrange
            var userRes = new UserRes
            {
                Id = 0,
                FirstName = null,
                LastName = null,
                Email = null,
                UserType = UserType.Member
            };

            // Act
            var validationContext = new ValidationContext(userRes);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(userRes, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("A")]
        [InlineData("Very Long First Name")]
        public void UserRes_FirstName_AcceptsVariousValues(string firstName)
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                FirstName = firstName
            };

            // Assert
            Assert.Equal(firstName, userRes.FirstName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("B")]
        [InlineData("Very-Long-Last-Name-With-Hyphens")]
        public void UserRes_LastName_AcceptsVariousValues(string lastName)
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                LastName = lastName
            };

            // Assert
            Assert.Equal(lastName, userRes.LastName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("user@example.com")]
        [InlineData("complex.email+tag@long-domain.co.uk")]
        [InlineData("invalid-email-format")]  // No validation on UserRes
        public void UserRes_Email_AcceptsVariousValues(string email)
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                Email = email
            };

            // Assert
            Assert.Equal(email, userRes.Email);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999999)]
        public void UserRes_Id_AcceptsVariousValues(int id)
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                Id = id
            };

            // Assert
            Assert.Equal(id, userRes.Id);
        }

        [Fact]
        public void UserRes_CompleteUserData_SetsAllProperties()
        {
            // Arrange & Act
            var userRes = new UserRes
            {
                Id = 100,
                FirstName = "Complete",
                LastName = "User",
                Email = "complete.user@test.com",
                UserType = UserType.Host
            };

            // Assert
            Assert.Equal(100, userRes.Id);
            Assert.Equal("Complete", userRes.FirstName);
            Assert.Equal("User", userRes.LastName);
            Assert.Equal("complete.user@test.com", userRes.Email);
            Assert.Equal(UserType.Host, userRes.UserType);
        }

        [Fact]
        public void UserRes_NoValidationAttributes_PassesValidation()
        {
            // Arrange
            var userRes = new UserRes
            {
                Id = -1,
                FirstName = "",
                LastName = "",
                Email = "not-an-email",
                UserType = UserType.Member
            };

            // Act
            var validationContext = new ValidationContext(userRes);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(userRes, validationContext, validationResults, true);

            // Assert - UserRes has no validation attributes, so everything should pass
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UserRes_CanBeUsedInLoginResponse()
        {
            // Arrange
            var userRes = new UserRes
            {
                Id = 456,
                FirstName = "Response",
                LastName = "Test",
                Email = "response.test@example.com",
                UserType = UserType.Admin
            };

            var loginResponse = new LoginResponseModel
            {
                Token = "jwt.token.here",
                SignedInUser = userRes
            };

            // Act & Assert
            Assert.Equal(userRes, loginResponse.SignedInUser);
            Assert.Equal(456, loginResponse.SignedInUser.Id);
            Assert.Equal("Response", loginResponse.SignedInUser.FirstName);
            Assert.Equal("Test", loginResponse.SignedInUser.LastName);
            Assert.Equal("response.test@example.com", loginResponse.SignedInUser.Email);
            Assert.Equal(UserType.Admin, loginResponse.SignedInUser.UserType);
        }

        [Fact]
        public void UserRes_DefaultEnumValue_IsAdmin()
        {
            // Arrange & Act
            var userRes = new UserRes();

            // Assert
            Assert.Equal(UserType.Admin, userRes.UserType);
        }

        [Fact]
        public void UserRes_EqualsAndHashCode_WorksForComparison()
        {
            // Arrange
            var userRes1 = new UserRes
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                UserType = UserType.Member
            };

            var userRes2 = new UserRes
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                UserType = UserType.Member
            };

            var userRes3 = new UserRes
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                UserType = UserType.Host
            };

            // Act & Assert
            // Note: These are reference types, so they won't be equal unless it's the same instance
            Assert.NotEqual(userRes1, userRes2); // Different instances
            Assert.NotEqual(userRes1, userRes3); // Different data
            Assert.Equal(userRes1, userRes1); // Same instance
        }
    }
}