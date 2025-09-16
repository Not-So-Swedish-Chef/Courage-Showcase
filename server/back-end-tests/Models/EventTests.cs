using back_end.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace back_end.Tests.Models
{
    public class EventTests
    {
        [Fact]
        public void Event_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var eventObj = new Event();

            // Assert
            Assert.True(eventObj.StartDateTime > DateTime.MinValue);
            Assert.True(eventObj.EndDateTime > DateTime.MinValue);
            Assert.True(eventObj.EndDateTime > eventObj.StartDateTime);
            Assert.Equal("", eventObj.Title);
            Assert.Equal("", eventObj.Location);
            Assert.Equal("", eventObj.ImageUrl);
            Assert.Equal("", eventObj.Url);
            Assert.Equal(0, eventObj.Price);
            Assert.NotNull(eventObj.UsersWhoSaved);
            Assert.Empty(eventObj.UsersWhoSaved);
        }

        [Fact]
        public void Event_SetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            var eventObj = new Event();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(1).AddHours(2);

            // Act
            eventObj.Id = 1;
            eventObj.Title = "Test Event";
            eventObj.Location = "Test Location";
            eventObj.ImageUrl = "https://test.com/image.jpg";
            eventObj.StartDateTime = startDate;
            eventObj.EndDateTime = endDate;
            eventObj.Price = 50.99m;
            eventObj.Url = "https://test.com";
            eventObj.HostId = 1;

            // Assert
            Assert.Equal(1, eventObj.Id);
            Assert.Equal("Test Event", eventObj.Title);
            Assert.Equal("Test Location", eventObj.Location);
            Assert.Equal("https://test.com/image.jpg", eventObj.ImageUrl);
            Assert.Equal(startDate, eventObj.StartDateTime);
            Assert.Equal(endDate, eventObj.EndDateTime);
            Assert.Equal(50.99m, eventObj.Price);
            Assert.Equal("https://test.com", eventObj.Url);
            Assert.Equal(1, eventObj.HostId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Event_RequiredFields_ValidationFailsForInvalidValues(string invalidValue)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = invalidValue,
                Location = invalidValue,
                HostId = 1
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.Title)));
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.Location)));
        }

        [Fact]
        public void Event_ValidModel_PassesValidation()
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Valid Title",
                Location = "Valid Location",
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                HostId = 1,
                Price = 25.50m,
                Url = "https://valid-url.com"
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Event_NegativePrice_ValidationFails(decimal negativePrice)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                Price = negativePrice
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.Price)));
        }

        [Theory]
        [InlineData("invalid-url")]
        [InlineData("not a url")]
        public void Event_InvalidUrl_ValidationFails(string invalidUrl)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                Url = invalidUrl
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.Url)));
        }

        [Theory]
        [InlineData("https://valid-url.com")]
        [InlineData("http://also-valid.com")]
        public void Event_ValidUrl_PassesValidation(string validUrl)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                Url = validUrl
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(Event.Url)));
        }

        [Fact]
        public void Event_StartDateAfterEndDate_CustomValidationFails()
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                StartDateTime = DateTime.UtcNow.AddDays(2),
                EndDateTime = DateTime.UtcNow.AddDays(1) // End before start
            };

            var validationContext = new ValidationContext(eventObj);

            // Act
            var customValidationResults = eventObj.Validate(validationContext);

            // Assert
            Assert.Single(customValidationResults);
            Assert.Contains("Start date and time must be before end date and time.",
                customValidationResults.First().ErrorMessage);
        }

        [Fact]
        public void Event_StartDateBeforeEndDate_CustomValidationPasses()
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(2)
            };

            var validationContext = new ValidationContext(eventObj);

            // Act
            var customValidationResults = eventObj.Validate(validationContext);

            // Assert
            Assert.Empty(customValidationResults);
        }

        [Fact]
        public void Event_UsersWhoSaved_CanAddUsers()
        {
            // Arrange
            var eventObj = new Event();
            var user1 = new User { FirstName = "John", LastName = "Doe" };
            var user2 = new User { FirstName = "Jane", LastName = "Smith" };

            // Act
            eventObj.UsersWhoSaved.Add(user1);
            eventObj.UsersWhoSaved.Add(user2);

            // Assert
            Assert.Equal(2, eventObj.UsersWhoSaved.Count);
            Assert.Contains(user1, eventObj.UsersWhoSaved);
            Assert.Contains(user2, eventObj.UsersWhoSaved);
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}