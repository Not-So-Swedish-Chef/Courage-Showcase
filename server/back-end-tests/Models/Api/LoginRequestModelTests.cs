using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using back_end.Models.Api;

namespace back_end.Tests.Models.Api
{
    public class LoginRequestModelTests
    {
        [Fact]
        public void LoginRequestModel_DefaultConstructor_InitializesWithNullValues()
        {
            // Act
            var model = new LoginRequestModel();

            // Assert
            Assert.Null(model.Email);
            Assert.Null(model.Password);
        }

        [Fact]
        public void LoginRequestModel_SetProperties_SetsCorrectly()
        {
            // Arrange & Act
            var model = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = "SecurePassword123!"
            };

            // Assert
            Assert.Equal("test@example.com", model.Email);
            Assert.Equal("SecurePassword123!", model.Password);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void LoginRequestModel_RequiredEmail_FailsValidation(string email)
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = email,
                Password = "ValidPassword123!"
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void LoginRequestModel_RequiredPassword_FailsValidation(string password)
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Password"));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("not.an.email")]
        [InlineData("missing@")]
        [InlineData("@missing.com")]
        [InlineData("no-domain@")]
        [InlineData("test@")]
        public void LoginRequestModel_InvalidEmailFormat_FailsValidation(string invalidEmail)
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = invalidEmail,
                Password = "ValidPassword123!"
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("valid+email@test-domain.org")]
        [InlineData("simple@test.net")]
        [InlineData("complex.email+tag@long-domain-name.com")]
        [InlineData("123@numbers.com")]
        [InlineData("test_email@underscores.net")]
        public void LoginRequestModel_ValidEmailFormat_PassesEmailValidation(string validEmail)
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = validEmail,
                Password = "ValidPassword123!"
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("password")]
        [InlineData("123456")]
        [InlineData("Password123!")]
        [InlineData("a")]
        [InlineData("VeryLongPasswordThatShouldStillBeValid123456789!@#$%")]
        public void LoginRequestModel_AnyPassword_PassesValidation(string password)
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginRequestModel_ValidModel_PassesAllValidations()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "user@example.com",
                Password = "SecurePassword123!"
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
        public void LoginRequestModel_BothFieldsNull_FailsValidation()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = null,
                Password = null
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Password"));
            Assert.Equal(2, validationResults.Count);
        }

        [Fact]
        public void LoginRequestModel_BothFieldsEmpty_FailsValidation()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "",
                Password = ""
            };

            // Act
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginRequestModel_EmailSpecialCharacters_HandledCorrectly()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "user+test@example-domain.co.uk",
                Password = "password"
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
        public void LoginRequestModel_PasswordWithSpecialCharacters_HandledCorrectly()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = "P@ssw0rd!#$%^&*()_+-=[]{}|;':\",./<>?"
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
        public void LoginRequestModel_CaseSensitive_EmailAndPassword()
        {
            // Arrange
            var model1 = new LoginRequestModel
            {
                Email = "Test@Example.Com",
                Password = "Password123"
            };

            var model2 = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act & Assert
            Assert.NotEqual(model1.Email, model2.Email);
            Assert.NotEqual(model1.Password, model2.Password);

            // Both should still be valid
            var validationContext1 = new ValidationContext(model1);
            var validationResults1 = new List<ValidationResult>();
            var isValid1 = Validator.TryValidateObject(model1, validationContext1, validationResults1, true);

            var validationContext2 = new ValidationContext(model2);
            var validationResults2 = new List<ValidationResult>();
            var isValid2 = Validator.TryValidateObject(model2, validationContext2, validationResults2, true);

            Assert.True(isValid1);
            Assert.True(isValid2);
        }
    }
}