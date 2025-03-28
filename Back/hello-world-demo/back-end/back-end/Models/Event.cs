﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string ImageUrl { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string CategoryId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; } // Nullable to allow skipping when IsFree is true

        public bool IsFree { get; set; }
 
        public string Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Ensure StartDateTime is before EndDateTime
            if (StartDateTime >= EndDateTime)
            {
                results.Add(new ValidationResult("Start date and time must be before end date and time.", new[] { nameof(StartDateTime), nameof(EndDateTime) }));
            }

            // Ensure Price is required only if IsFree is false
            if (!IsFree && (!Price.HasValue || Price <= 0))
            {
                results.Add(new ValidationResult("Price must be a positive number when the event is not free.", new[] { nameof(Price) }));
            }
            return results;
        }
    }
}
