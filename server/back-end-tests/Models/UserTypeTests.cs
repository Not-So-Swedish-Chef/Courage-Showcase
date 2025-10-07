using Xunit;
using System;
using System.Linq;
using back_end.Models;

namespace back_end.Tests.Models
{
    public class UserTypeTests
    {
        [Fact]
        public void UserType_HasExpectedValues()
        {
            // Act & Assert
            var userTypeValues = Enum.GetValues(typeof(UserType)).Cast<UserType>().ToArray();

            Assert.Contains(UserType.Admin, userTypeValues);
            Assert.Contains(UserType.Host, userTypeValues);
            Assert.Contains(UserType.Member, userTypeValues);
            Assert.Equal(3, userTypeValues.Length);
        }

        [Fact]
        public void UserType_Admin_HasCorrectValue()
        {
            // Act
            int adminValue = (int)UserType.Admin;

            // Assert
            Assert.Equal(0, adminValue);
        }

        [Fact]
        public void UserType_Host_HasCorrectValue()
        {
            // Act
            int hostValue = (int)UserType.Host;

            // Assert
            Assert.Equal(1, hostValue);
        }

        [Fact]
        public void UserType_Member_HasCorrectValue()
        {
            // Act
            int memberValue = (int)UserType.Member;

            // Assert
            Assert.Equal(2, memberValue);
        }

        [Theory]
        [InlineData(UserType.Admin, "Admin")]
        [InlineData(UserType.Host, "Host")]
        [InlineData(UserType.Member, "Member")]
        public void UserType_ToString_ReturnsCorrectString(UserType userType, string expected)
        {
            // Act
            string result = userType.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Admin", UserType.Admin)]
        [InlineData("Host", UserType.Host)]
        [InlineData("Member", UserType.Member)]
        public void UserType_Parse_ConvertsStringCorrectly(string input, UserType expected)
        {
            // Act
            var result = Enum.Parse<UserType>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("admin", UserType.Admin)]
        [InlineData("host", UserType.Host)]
        [InlineData("member", UserType.Member)]
        [InlineData("ADMIN", UserType.Admin)]
        [InlineData("HOST", UserType.Host)]
        [InlineData("MEMBER", UserType.Member)]
        public void UserType_ParseIgnoreCase_ConvertsStringCorrectly(string input, UserType expected)
        {
            // Act
            var result = Enum.Parse<UserType>(input, ignoreCase: true);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Admin", true, UserType.Admin)]
        [InlineData("Host", true, UserType.Host)]
        [InlineData("Member", true, UserType.Member)]
        [InlineData("InvalidType", false, default(UserType))]
        [InlineData("", false, default(UserType))]
        [InlineData(null, false, default(UserType))]
        public void UserType_TryParse_HandlesValidAndInvalidStrings(string input, bool expectedSuccess, UserType expectedValue)
        {
            // Act
            bool success = Enum.TryParse<UserType>(input, out UserType result);

            // Assert
            Assert.Equal(expectedSuccess, success);
            if (expectedSuccess)
            {
                Assert.Equal(expectedValue, result);
            }
        }

        [Fact]
        public void UserType_IsDefined_ReturnsTrueForValidValues()
        {
            // Act & Assert
            Assert.True(Enum.IsDefined(typeof(UserType), UserType.Admin));
            Assert.True(Enum.IsDefined(typeof(UserType), UserType.Host));
            Assert.True(Enum.IsDefined(typeof(UserType), UserType.Member));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        [InlineData(99)]
        public void UserType_IsDefined_ReturnsFalseForInvalidValues(int invalidValue)
        {
            // Act & Assert
            Assert.False(Enum.IsDefined(typeof(UserType), invalidValue));
        }

        [Fact]
        public void UserType_GetNames_ReturnsAllEnumNames()
        {
            // Act
            string[] names = Enum.GetNames(typeof(UserType));

            // Assert
            Assert.Equal(3, names.Length);
            Assert.Contains("Admin", names);
            Assert.Contains("Host", names);
            Assert.Contains("Member", names);
        }

        [Fact]
        public void UserType_GetValues_ReturnsAllEnumValues()
        {
            // Act
            Array values = Enum.GetValues(typeof(UserType));

            // Assert
            Assert.Equal(3, values.Length);
            Assert.Contains(UserType.Admin, values.Cast<UserType>());
            Assert.Contains(UserType.Host, values.Cast<UserType>());
            Assert.Contains(UserType.Member, values.Cast<UserType>());
        }

        [Fact]
        public void UserType_CanBeUsedInSwitch()
        {
            // Arrange
            var userType = UserType.Host;
            string result;

            // Act
            switch (userType)
            {
                case UserType.Admin:
                    result = "Administrator";
                    break;
                case UserType.Host:
                    result = "Event Host";
                    break;
                case UserType.Member:
                    result = "Regular Member";
                    break;
                default:
                    result = "Unknown";
                    break;
            }

            // Assert
            Assert.Equal("Event Host", result);
        }

        [Fact]
        public void UserType_CanBeCompared()
        {
            // Arrange & Act & Assert
            Assert.True(UserType.Admin < UserType.Host);
            Assert.True(UserType.Host < UserType.Member);
            Assert.True(UserType.Admin < UserType.Member);

            Assert.False(UserType.Host < UserType.Admin);
            Assert.False(UserType.Member < UserType.Host);
            Assert.False(UserType.Member < UserType.Admin);

            Assert.True(UserType.Admin == UserType.Admin);
            Assert.True(UserType.Host == UserType.Host);
            Assert.True(UserType.Member == UserType.Member);

            Assert.False(UserType.Admin == UserType.Host);
            Assert.False(UserType.Host == UserType.Member);
            Assert.False(UserType.Admin == UserType.Member);
        }

        [Theory]
        [InlineData(UserType.Admin)]
        [InlineData(UserType.Host)]
        [InlineData(UserType.Member)]
        public void UserType_CanBeAssignedToVariable(UserType userType)
        {
            // Act
            UserType assignedType = userType;

            // Assert
            Assert.Equal(userType, assignedType);
        }
    }
}