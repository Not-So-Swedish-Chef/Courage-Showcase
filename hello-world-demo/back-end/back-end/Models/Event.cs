using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = "";

        public string Location { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public DateTime StartDateTime { get; set; } 

        public DateTime EndDateTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public string Url { get; set; } = "";
        public int HostId { get; set; }

        public virtual Host Host { get; set; } = new Host();
        public ICollection<User> UsersWhoSaved { get; set; } = new List<User>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            
            // Ensure StartDateTime is before EndDateTime
            if (StartDateTime >= EndDateTime)
            {
                results.Add(new ValidationResult("Start date and time must be before end date and time.", new[] { nameof(StartDateTime), nameof(EndDateTime) }));
            }
            return results;
        }
    }
}
