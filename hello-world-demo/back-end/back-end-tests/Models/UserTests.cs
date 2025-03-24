using System.ComponentModel.DataAnnotations;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class UserTests
    {
        [Fact]
        public void Validate_ReturnsNoErrors_WhenUserIsValid()
        {
            // Arrange
            var validUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
            };

            var validationContext = new ValidationContext(validUser);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(validUser, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

       
    }
}