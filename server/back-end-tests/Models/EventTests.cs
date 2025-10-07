using back_end.Models;
using back_end.Enums;
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
            Assert.Equal(OntarioCity.Toronto, eventObj.City);
            Assert.Equal(EventStatus.Active, eventObj.Status);
            Assert.NotNull(eventObj.UsersWhoSaved);
            Assert.Empty(eventObj.UsersWhoSaved);
            Assert.NotNull(eventObj.DisabilityTags);
            Assert.Empty(eventObj.DisabilityTags);
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
            eventObj.City = OntarioCity.Ottawa;
            eventObj.ImageUrl = "https://test.com/image.jpg";
            eventObj.StartDateTime = startDate;
            eventObj.EndDateTime = endDate;
            eventObj.Price = 50.99m;
            eventObj.Url = "https://test.com";
            eventObj.HostId = 1;
            eventObj.Status = EventStatus.Active;
            eventObj.MinAge = 18;
            eventObj.MaxAge = 65;

            // Assert
            Assert.Equal(1, eventObj.Id);
            Assert.Equal("Test Event", eventObj.Title);
            Assert.Equal("Test Location", eventObj.Location);
            Assert.Equal(OntarioCity.Ottawa, eventObj.City);
            Assert.Equal("https://test.com/image.jpg", eventObj.ImageUrl);
            Assert.Equal(startDate, eventObj.StartDateTime);
            Assert.Equal(endDate, eventObj.EndDateTime);
            Assert.Equal(50.99m, eventObj.Price);
            Assert.Equal("https://test.com", eventObj.Url);
            Assert.Equal(1, eventObj.HostId);
            Assert.Equal(EventStatus.Active, eventObj.Status);
            Assert.Equal(18, eventObj.MinAge);
            Assert.Equal(65, eventObj.MaxAge);
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
            Assert.Contains("Start date/time must be before end date/time.",
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

        [Fact]
        public void Event_DisabilityTags_CanAddTags()
        {
            // Arrange
            var eventObj = new Event();
            var tag1 = new DisabilityTag { Id = 1, Name = "Wheelchair Accessible" };
            var tag2 = new DisabilityTag { Id = 2, Name = "ASL Interpreter" };

            // Act
            eventObj.DisabilityTags.Add(tag1);
            eventObj.DisabilityTags.Add(tag2);

            // Assert
            Assert.Equal(2, eventObj.DisabilityTags.Count);
            Assert.Contains(tag1, eventObj.DisabilityTags);
            Assert.Contains(tag2, eventObj.DisabilityTags);
        }

        [Fact]
        public void Event_MinAgeGreaterThanMaxAge_CustomValidationFails()
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(2),
                MinAge = 65,
                MaxAge = 18 // Min > Max
            };

            var validationContext = new ValidationContext(eventObj);

            // Act
            var customValidationResults = eventObj.Validate(validationContext);

            // Assert
            Assert.Single(customValidationResults);
            Assert.Contains("MinAge cannot be greater than MaxAge.",
                customValidationResults.First().ErrorMessage);
        }

        [Fact]
        public void Event_ValidAgeRange_CustomValidationPasses()
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(2),
                MinAge = 18,
                MaxAge = 65
            };

            var validationContext = new ValidationContext(eventObj);

            // Act
            var customValidationResults = eventObj.Validate(validationContext);

            // Assert
            Assert.Empty(customValidationResults);
        }

        [Theory]
        [InlineData(151)]
        [InlineData(200)]
        public void Event_InvalidMinAge_ValidationFails(int invalidAge)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                MinAge = invalidAge
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.MinAge)));
        }

        [Theory]
        [InlineData(151)]
        [InlineData(200)]
        public void Event_InvalidMaxAge_ValidationFails(int invalidAge)
        {
            // Arrange
            var eventObj = new Event
            {
                Title = "Test Event",
                Location = "Test Location",
                HostId = 1,
                MaxAge = invalidAge
            };

            // Act
            var validationResults = ValidateModel(eventObj);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Event.MaxAge)));
        }

        [Fact]
        public void Event_StatusEnum_CanBeSet()
        {
            // Arrange
            var eventObj = new Event();

            // Act
            eventObj.Status = EventStatus.Expired;

            // Assert
            Assert.Equal(EventStatus.Expired, eventObj.Status);
        }

        [Fact]
        public void Event_CityEnum_CanBeSet()
        {
            // Arrange
            var eventObj = new Event();

            // Act
            eventObj.City = OntarioCity.Hamilton;

            // Assert
            Assert.Equal(OntarioCity.Hamilton, eventObj.City);
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