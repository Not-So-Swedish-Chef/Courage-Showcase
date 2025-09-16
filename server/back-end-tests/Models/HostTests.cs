using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using back_end.Models;

namespace back_end.Tests.Models
{
    public class HostTests
    {
        [Fact]
        public void Host_DefaultConstructor_InitializesProperties()
        {
            // Act
            var host = new Host();

            // Assert
            Assert.Equal(0, host.Id);
            Assert.Null(host.AgencyName);
            Assert.Null(host.Bio);
            Assert.NotNull(host.User);
            Assert.NotNull(host.Events);
            Assert.Empty(host.Events);
        }

        [Fact]
        public void Host_SetProperties_SetsCorrectly()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserType = UserType.Host
            };

            // Act
            var host = new Host
            {
                Id = 1,
                AgencyName = "Event Masters Inc.",
                Bio = "Professional event organizer with 10+ years experience",
                User = user
            };

            // Assert
            Assert.Equal(1, host.Id);
            Assert.Equal("Event Masters Inc.", host.AgencyName);
            Assert.Equal("Professional event organizer with 10+ years experience", host.Bio);
            Assert.Equal(user, host.User);
            Assert.Equal("John", host.User.FirstName);
            Assert.Equal("Doe", host.User.LastName);
            Assert.Equal(UserType.Host, host.User.UserType);
        }

        [Fact]
        public void Host_AgencyName_CanBeNull()
        {
            // Arrange & Act
            var host = new Host
            {
                Id = 1,
                AgencyName = null,
                Bio = "Some bio"
            };

            // Assert
            Assert.Null(host.AgencyName);

            // Validate that null AgencyName doesn't cause validation errors
            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Fact]
        public void Host_Bio_CanBeNull()
        {
            // Arrange & Act
            var host = new Host
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = null
            };

            // Assert
            Assert.Null(host.Bio);

            // Validate that null Bio doesn't cause validation errors
            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("A")]
        [InlineData("Very Long Agency Name That Could Be Used For Testing Purposes")]
        public void Host_AgencyName_AcceptsVariousValidStrings(string agencyName)
        {
            // Arrange & Act
            var host = new Host
            {
                Id = 1,
                AgencyName = agencyName,
                Bio = "Test bio"
            };

            // Assert
            Assert.Equal(agencyName, host.AgencyName);

            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Short bio")]
        [InlineData("This is a very long bio that describes the host in great detail, including their experience, specialties, and background information that potential clients might find useful when deciding whether to book their services.")]
        public void Host_Bio_AcceptsVariousValidStrings(string bio)
        {
            // Arrange & Act
            var host = new Host
            {
                Id = 1,
                AgencyName = "Test Agency",
                Bio = bio
            };

            // Assert
            Assert.Equal(bio, host.Bio);

            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Fact]
        public void Host_Events_CanAddEvents()
        {
            // Arrange
            var host = new Host();
            var event1 = new Event
            {
                Title = "Event 1",
                Location = "Location 1",
                HostId = host.Id
            };
            var event2 = new Event
            {
                Title = "Event 2",
                Location = "Location 2",
                HostId = host.Id
            };

            // Act
            host.Events.Add(event1);
            host.Events.Add(event2);

            // Assert
            Assert.Equal(2, host.Events.Count);
            Assert.Contains(event1, host.Events);
            Assert.Contains(event2, host.Events);
        }

        [Fact]
        public void Host_Events_CanRemoveEvents()
        {
            // Arrange
            var host = new Host();
            var event1 = new Event
            {
                Title = "Event 1",
                Location = "Location 1",
                HostId = host.Id
            };
            var event2 = new Event
            {
                Title = "Event 2",
                Location = "Location 2",
                HostId = host.Id
            };

            host.Events.Add(event1);
            host.Events.Add(event2);

            // Act
            host.Events.Remove(event1);

            // Assert
            Assert.Single(host.Events);
            Assert.DoesNotContain(event1, host.Events);
            Assert.Contains(event2, host.Events);
        }

        [Fact]
        public void Host_User_NavigationProperty_WorksCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                UserType = UserType.Host
            };

            // Act
            var host = new Host
            {
                Id = 1,
                User = user,
                AgencyName = "Smith Events",
                Bio = "Experienced event planner"
            };

            // Assert
            Assert.Equal(user, host.User);
            Assert.Equal(1, host.User.Id);
            Assert.Equal("Jane", host.User.FirstName);
            Assert.Equal("Smith", host.User.LastName);
            Assert.Equal("jane.smith@example.com", host.User.Email);
            Assert.Equal(UserType.Host, host.User.UserType);
        }

        [Fact]
        public void Host_IdAsForeignKey_MatchesUserId()
        {
            // Arrange
            var userId = 123;
            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                UserType = UserType.Host
            };

            // Act
            var host = new Host
            {
                Id = userId,
                User = user,
                AgencyName = "Test Agency"
            };

            // Assert
            Assert.Equal(userId, host.Id);
            Assert.Equal(userId, host.User.Id);
            Assert.Equal(host.Id, host.User.Id);
        }

        [Fact]
        public void Host_CompleteHostProfile_PassesValidation()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Professional",
                LastName = "Organizer",
                Email = "pro@events.com",
                UserType = UserType.Host
            };

            var host = new Host
            {
                Id = 1,
                User = user,
                AgencyName = "Premier Events LLC",
                Bio = "Award-winning event planning company specializing in corporate events and weddings. Over 15 years of experience creating memorable experiences."
            };

            // Act
            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Host_MinimalHostProfile_PassesValidation()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Simple",
                LastName = "Host",
                UserType = UserType.Host
            };

            var host = new Host
            {
                Id = 1,
                User = user,
                AgencyName = null,
                Bio = null
            };

            // Act
            var validationContext = new ValidationContext(host);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(host, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}