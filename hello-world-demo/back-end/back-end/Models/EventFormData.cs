using System;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class EventFormData : IValidatableObject
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Start date and time is required")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End date and time is required")]
        public DateTime EndDateTime { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public string CategoryId { get; set; }

        public decimal? Price { get; set; } // Nullable to allow skipping when IsFree is true

        [Required(ErrorMessage = "IsFree is required")]
        public bool IsFree { get; set; }

        [Required(ErrorMessage = "URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
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
