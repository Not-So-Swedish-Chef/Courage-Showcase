using back_end.Enums;
using Xunit;

namespace back_end_tests.Enums
{
    public class EventStatusTests
    {
        [Fact]
        public void EventStatus_HasActiveValue()
        {
            // Act
            var status = EventStatus.Active;

            // Assert
            Assert.Equal(0, (int)status);
        }

        [Fact]
        public void EventStatus_HasExpiredValue()
        {
            // Act
            var status = EventStatus.Expired;

            // Assert
            Assert.Equal(1, (int)status);
        }

        [Fact]
        public void EventStatus_HasCanceledValue()
        {
            // Act
            var status = EventStatus.Canceled;

            // Assert
            Assert.Equal(2, (int)status);
        }

        [Fact]
        public void EventStatus_CanCompareValues()
        {
            // Arrange
            var active = EventStatus.Active;
            var expired = EventStatus.Expired;
            var canceled = EventStatus.Canceled;

            // Assert
            Assert.NotEqual(active, expired);
            Assert.NotEqual(active, canceled);
            Assert.NotEqual(expired, canceled);
        }

        [Theory]
        [InlineData(0, EventStatus.Active)]
        [InlineData(1, EventStatus.Expired)]
        [InlineData(2, EventStatus.Canceled)]
        public void EventStatus_CanCastFromInt(int intValue, EventStatus expectedStatus)
        {
            // Act
            var status = (EventStatus)intValue;

            // Assert
            Assert.Equal(expectedStatus, status);
        }

        [Fact]
        public void EventStatus_DefaultValue_IsActive()
        {
            // Arrange & Act
            EventStatus status = default;

            // Assert
            Assert.Equal(EventStatus.Active, status);
        }
    }
}


