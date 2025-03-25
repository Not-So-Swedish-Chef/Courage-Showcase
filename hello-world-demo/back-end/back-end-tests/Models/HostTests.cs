using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class HostTests
    {
        [Fact]
        public void Host_IsValid_WhenRequiredFieldsAreSet()
        {
            var host = new Host
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = "Host bio",
                User = new User { FirstName = "Test", LastName = "User", Email = "host@example.com" },
                Events = new List<Event>()
            };

            var context = new ValidationContext(host);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(host, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
