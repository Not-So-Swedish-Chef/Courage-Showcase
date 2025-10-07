using System.ComponentModel.DataAnnotations;
using Xunit;

namespace back_end.Tests.Models
{
    public class DisabilityTagTests
    {
        [Fact]
        public void DisabilityTag_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var tag = new DisabilityTag();

            // Assert
            Assert.Equal(0, tag.Id);
            Assert.Equal("", tag.Name);
            Assert.Equal("", tag.NormalizedName);
            Assert.NotNull(tag.Events);
            Assert.Empty(tag.Events);
        }

        [Fact]
        public void DisabilityTag_SetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            var tag = new DisabilityTag();

            // Act
            tag.Id = 1;
            tag.Name = "Wheelchair Accessible";
            tag.NormalizedName = "WHEELCHAIR ACCESSIBLE";

            // Assert
            Assert.Equal(1, tag.Id);
            Assert.Equal("Wheelchair Accessible", tag.Name);
            Assert.Equal("WHEELCHAIR ACCESSIBLE", tag.NormalizedName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DisabilityTag_RequiredName_ValidationFailsForInvalidValues(string invalidValue)
        {
            // Arrange
            var tag = new DisabilityTag
            {
                Name = invalidValue
            };

            // Act
            var validationResults = ValidateModel(tag);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(DisabilityTag.Name)));
        }

        [Fact]
        public void DisabilityTag_ValidName_PassesValidation()
        {
            // Arrange
            var tag = new DisabilityTag
            {
                Name = "ASL Interpreter"
            };

            // Act
            var validationResults = ValidateModel(tag);

            // Assert
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(DisabilityTag.Name)));
        }

        [Fact]
        public void DisabilityTag_NameExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var tag = new DisabilityTag
            {
                Name = new string('A', 65) // Max length is 64
            };

            // Act
            var validationResults = ValidateModel(tag);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(DisabilityTag.Name)));
        }

        [Fact]
        public void DisabilityTag_NormalizedNameExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var tag = new DisabilityTag
            {
                Name = "Valid Name",
                NormalizedName = new string('A', 65) // Max length is 64
            };

            // Act
            var validationResults = ValidateModel(tag);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(DisabilityTag.NormalizedName)));
        }

        [Fact]
        public void DisabilityTag_Events_CanAddEvents()
        {
            // Arrange
            var tag = new DisabilityTag { Id = 1, Name = "Wheelchair Accessible" };
            var event1 = new Event { Id = 1, Title = "Event 1", Location = "Location 1" };
            var event2 = new Event { Id = 2, Title = "Event 2", Location = "Location 2" };

            // Act
            tag.Events.Add(event1);
            tag.Events.Add(event2);

            // Assert
            Assert.Equal(2, tag.Events.Count);
            Assert.Contains(event1, tag.Events);
            Assert.Contains(event2, tag.Events);
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


