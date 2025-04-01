using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class EventTests
    {
        [Fact]
        public void Validate_ReturnsNoErrors_WhenEventIsValid()
        {
            // Arrange
            var validEvent = new Event
            {
                Title = "Valid Event",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(3),
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = validEvent.Validate(new ValidationContext(validEvent)).ToList().Count == 0;

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Validate_ReturnsError_WhenStartDateTimeIsAfterEndDateTime()
        {
            // Arrange
            var invalidEvent = new Event
            {
                Title = "Invalid Event",
                StartDateTime = DateTime.Now.AddHours(3),
                EndDateTime = DateTime.Now.AddHours(1),
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var errors = invalidEvent.Validate(new ValidationContext(invalidEvent)).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Contains("Start date and time must be before end date and time.", errors[0].ErrorMessage);
        }

        [Fact]
        public void Validate_ReturnsError_WhenPriceIsMissingForPaidEvent()
        {
            // Arrange
            var invalidEvent = new Event
            {
                Title = "Invalid Event",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(3),
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var errors = invalidEvent.Validate(new ValidationContext(invalidEvent)).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Contains("Price must be a positive number when the event is not free.", errors[0].ErrorMessage);
        }

        [Fact]
        public void Validate_ReturnsError_WhenPriceIsZeroOrNegativeForPaidEvent()
        {
            // Arrange
            var invalidEvent = new Event
            {
                Title = "Invalid Event",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(3),
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var errors = invalidEvent.Validate(new ValidationContext(invalidEvent)).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Contains("Price must be a positive number when the event is not free.", errors[0].ErrorMessage);
        }

        [Fact]
        public void Validate_ReturnsNoErrors_WhenPriceIsValidForPaidEvent()
        {
            // Arrange
            var validEvent = new Event
            {
                Title = "Valid Event",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(3),
                Price = 10.50m // Price is valid
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var errors = validEvent.Validate(new ValidationContext(validEvent)).ToList();

            // Assert
            Assert.Empty(errors);
        }
    }
}