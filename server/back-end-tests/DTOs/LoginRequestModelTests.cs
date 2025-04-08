using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using back_end.Models.Api;
using Xunit;

namespace back_end_tests.DTOs
{
    public class LoginRequestModelTests
    {
        [Fact]
        public void LoginRequestModel_IsValid_WhenAllPropertiesAreSet()
        {
            // Arrange
            var model = new LoginRequestModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void LoginRequestModel_IsInvalid_WhenEmailIsMissing()
        {
            var model = new LoginRequestModel
            {
                Password = "password123"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(LoginRequestModel.Email)));
        }

        [Fact]
        public void LoginRequestModel_IsInvalid_WhenPasswordIsMissing()
        {
            var model = new LoginRequestModel
            {
                Email = "test@example.com"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(LoginRequestModel.Password)));
        }
    }
}
