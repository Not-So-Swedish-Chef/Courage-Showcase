using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using back_end.Models;

namespace back_end_tests.Models
{
    public class EventTests
    {
        private List<ValidationResult> ValidateModel(Event model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            results.AddRange(model.Validate(context));
            return results;
        }

        [Fact]
        public void Event_IsValid_WhenAllFieldsAreCorrect()
        {
            var model = new Event
            {
                Title = "Sample Event",
                Location = "Toronto",
                ImageUrl = "https://example.com/image.jpg",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(2),
                Price = 25,
                Url = "https://example.com",
                HostId = 1
            };

            var errors = ValidateModel(model);
            Assert.Empty(errors);
        }

        [Fact]
        public void Event_IsInvalid_WhenStartDateAfterEndDate()
        {
            var model = new Event
            {
                Title = "Event",
                Location = "Somewhere",
                StartDateTime = DateTime.Now.AddHours(3),
                EndDateTime = DateTime.Now.AddHours(1),
                Price = 10,
                HostId = 1
            };

            var errors = ValidateModel(model);
            Assert.Contains(errors, e => e.ErrorMessage.Contains("Start date and time must be before end date and time."));
        }


        [Fact]
        public void Event_IsInvalid_WhenPriceIsNegative()
        {
            var model = new Event
            {
                Title = "Free Event",
                Location = "Online",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(2),
                Price = -5,
                HostId = 1
            };

            var errors = ValidateModel(model);
            Assert.Contains(errors, e => e.ErrorMessage.Contains("Price must be a positive value."));
        }

        [Fact]
        public void Event_IsInvalid_WhenUrlIsInvalid()
        {
            var model = new Event
            {
                Title = "Event",
                Location = "City",
                StartDateTime = DateTime.Now.AddHours(1),
                EndDateTime = DateTime.Now.AddHours(2),
                Price = 10,
                Url = "not-a-url",
                HostId = 1
            };

            var errors = ValidateModel(model);
            Assert.Contains(errors, e => e.ErrorMessage.Contains("The Url field is not a valid fully-qualified http, https, or ftp URL."));
        }
    }
}