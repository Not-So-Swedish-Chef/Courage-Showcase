using Xunit;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using back_end.Models;

namespace back_end.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultConstructor_InitializesWithDefaults()
        {
            // Act
            var user = new User();

            // Assert
            Assert.Equal(0, user.Id);
            Assert.Null(user.FirstName);
            Assert.Equal("", user.LastName);
            Assert.Equal(UserType.Member, user.UserType);
            Assert.NotNull(user.SavedEvents);
            Assert.Empty(user.SavedEvents);
        }

        [Fact]
        public void User_SetProperties_SetsCorrectly()
        {
            // Act
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john.doe@example.com",
                UserType = UserType.Host
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("johndoe", user.UserName);
            Assert.Equal("john.doe@example.com", user.Email);
            Assert.Equal(UserType.Host, user.UserType);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void User_RequiredFirstName_FailsValidation(string firstName)
        {
            // Arrange
            var user = new User
            {
                FirstName = firstName,
                LastName = "Doe"
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("FirstName"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void User_RequiredLastName_FailsValidation(string lastName)
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = lastName
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("LastName"));
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Mary Jane")]
        [InlineData("José")]
        [InlineData("李")]
        [InlineData("O'Connor")]
        public void User_ValidFirstName_PassesValidation(string firstName)
        {
            // Arrange
            var user = new User
            {
                FirstName = firstName,
                LastName = "Doe"
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid || !validationResults.Any(vr => vr.MemberNames.Contains("FirstName")));
        }

        [Theory]
        [InlineData("Doe")]
        [InlineData("Smith-Johnson")]
        [InlineData("O'Brien")]
        [InlineData("Van Der Berg")]
        [InlineData("李")]
        public void User_ValidLastName_PassesValidation(string lastName)
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = lastName
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid || !validationResults.Any(vr => vr.MemberNames.Contains("LastName")));
        }

        [Theory]
        [InlineData(UserType.Admin)]
        [InlineData(UserType.Host)]
        [InlineData(UserType.Member)]
        public void User_UserType_CanBeSetToAnyValidType(UserType userType)
        {
            // Arrange & Act
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserType = userType
            };

            // Assert
            Assert.Equal(userType, user.UserType);
        }

        [Fact]
        public void User_DefaultUserType_IsMember()
        {
            // Act
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Assert
            Assert.Equal(UserType.Member, user.UserType);
        }

        [Fact]
        public void User_SavedEvents_CanAddEvents()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var event1 = new Event
            {
                Title = "Event 1",
                Location = "Location 1",
                HostId = 1
            };
            var event2 = new Event
            {
                Title = "Event 2",
                Location = "Location 2",
                HostId = 1
            };

            // Act
            user.SavedEvents.Add(event1);
            user.SavedEvents.Add(event2);

            // Assert
            Assert.Equal(2, user.SavedEvents.Count);
            Assert.Contains(event1, user.SavedEvents);
            Assert.Contains(event2, user.SavedEvents);
        }

        [Fact]
        public void User_SavedEvents_CanRemoveEvents()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var event1 = new Event
            {
                Title = "Event 1",
                Location = "Location 1",
                HostId = 1
            };
            var event2 = new Event
            {
                Title = "Event 2",
                Location = "Location 2",
                HostId = 1
            };

            user.SavedEvents.Add(event1);
            user.SavedEvents.Add(event2);

            // Act
            user.SavedEvents.Remove(event1);

            // Assert
            Assert.Single(user.SavedEvents);
            Assert.DoesNotContain(event1, user.SavedEvents);
            Assert.Contains(event2, user.SavedEvents);
        }

        [Fact]
        public void User_SavedEvents_InitializedAsEmptyCollection()
        {
            // Act
            var user = new User();

            // Assert
            Assert.NotNull(user.SavedEvents);
            Assert.Empty(user.SavedEvents);
            Assert.IsAssignableFrom<ICollection<Event>>(user.SavedEvents);
        }

        [Fact]
        public void User_IdentityProperties_CanBeSet()
        {
            // Arrange & Act
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe123",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = true
            };

            // Assert
            Assert.Equal("johndoe123", user.UserName);
            Assert.Equal("john.doe@example.com", user.Email);
            Assert.Equal("+1234567890", user.PhoneNumber);
            Assert.True(user.EmailConfirmed);
            Assert.False(user.PhoneNumberConfirmed);
            Assert.True(user.TwoFactorEnabled);
        }

        [Fact]
        public void User_CompleteUserProfile_PassesValidation()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                UserName = "janesmith",
                Email = "jane.smith@example.com",
                UserType = UserType.Host
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void User_MinimalValidUser_PassesValidation()
        {
            // Arrange
            var user = new User
            {
                FirstName = "A",
                LastName = "B"
            };

            // Act
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void User_LastNameDefaultValue_IsEmptyString()
        {
            // Act
            var user = new User
            {
                FirstName = "John"
            };

            // Assert
            Assert.Equal("", user.LastName);
        }

        [Fact]
        public void User_ManyToMany_SavedEventsRelationship()
        {
            // Arrange
            var user1 = new User { FirstName = "John", LastName = "Doe" };
            var user2 = new User { FirstName = "Jane", LastName = "Smith" };

            var event1 = new Event
            {
                Title = "Concert",
                Location = "Arena",
                HostId = 1
            };

            // Act
            user1.SavedEvents.Add(event1);
            user2.SavedEvents.Add(event1);
            event1.UsersWhoSaved.Add(user1);
            event1.UsersWhoSaved.Add(user2);

            // Assert
            Assert.Contains(event1, user1.SavedEvents);
            Assert.Contains(event1, user2.SavedEvents);
            Assert.Contains(user1, event1.UsersWhoSaved);
            Assert.Contains(user2, event1.UsersWhoSaved);
        }
    }
}