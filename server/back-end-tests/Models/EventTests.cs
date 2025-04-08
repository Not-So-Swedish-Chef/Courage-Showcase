using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using back_end.Models;
using Xunit;

namespace back_end_tests.Models
{
    public class EventTests
    {
        [Fact]
        public void Constructor_SetsDefaultStartAndEndDate()
        {
            // Act
            var evt = new Event();

            // Assert: allow a small range due to millisecond differences
            var duration = evt.EndDateTime - evt.StartDateTime;
            Assert.InRange(duration.TotalMinutes, 59, 61);
        }


        [Fact]
        public void Constructor_InitializesCollections()
        {
            var evt = new Event();

            Assert.NotNull(evt.UsersWhoSaved);
            Assert.Empty(evt.UsersWhoSaved);
        }

        [Fact]
        public void Validate_ReturnsError_WhenStartDateAfterEndDate()
        {
            var evt = new Event
            {
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow
            };

            var validationResults = evt.Validate(new ValidationContext(evt)).ToList();

            Assert.Single(validationResults);
            Assert.Contains("Start date and time must be before end date and time.", validationResults.First().ErrorMessage);
        }
    }
}
