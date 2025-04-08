using back_end.Models;
using Xunit;

namespace back_end_tests.Models
{
    public class UserTypeTests
    {
        [Theory]
        [InlineData(UserType.Admin, 0)]
        [InlineData(UserType.Host, 1)]
        [InlineData(UserType.Member, 2)]
        public void Enum_HasExpectedValues(UserType userType, int expectedValue)
        {
            // Act
            int actualValue = (int)userType;

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [InlineData(0, UserType.Admin)]
        [InlineData(1, UserType.Host)]
        [InlineData(2, UserType.Member)]
        public void Enum_CanBeParsedFromInt(int intValue, UserType expectedEnum)
        {
            // Act
            var parsed = (UserType)intValue;

            // Assert
            Assert.Equal(expectedEnum, parsed);
        }
    }
}
