using System;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class UserTypeTests
    {
        [Fact]
        public void UserType_ShouldContainExpectedValues()
        {
            var values = Enum.GetNames(typeof(UserType));
            Assert.Contains("Admin", values);
            Assert.Contains("Host", values);
            Assert.Contains("Member", values);
        }
    }
}
