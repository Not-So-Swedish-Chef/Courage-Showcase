using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_IsValid_WithRequiredFields()
        {
            var user = new User
            {
                FirstName = "Ahmed",
                LastName = "Muqarrib",
                Email = "test@example.com",
                UserType = UserType.Member
            };

            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(user, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void User_IsInvalid_WithoutFirstName()
        {
            var user = new User
            {
                LastName = "Muqarrib",
                Email = "test@example.com"
            };

            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(user, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }
    }
}
