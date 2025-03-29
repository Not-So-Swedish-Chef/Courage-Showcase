using back_end.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required]
    public string Location { get; set; } = "";

    public string ImageUrl { get; set; } = "";

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; } = 0;

    [Url]
    public string Url { get; set; } = "";

    // Foreign Key for Host
    [Required]
    public int HostId { get; set; }

    // Navigation Property
    [ForeignKey("HostId")]
    public virtual back_end.Models.Host? Host { get; set; }  // No default initialization

    // Many-to-Many relationship with UsersWhoSaved
    public ICollection<User> UsersWhoSaved { get; set; } = new List<User>();

    // Constructor to initialize default date values
    public Event()
    {
        StartDateTime = DateTime.UtcNow;
        EndDateTime = DateTime.UtcNow.AddHours(1);
    }

    // Validation Method
    //TODO: Move validation to appropriate class (EventValidator)
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (StartDateTime >= EndDateTime)
        {
            results.Add(new ValidationResult(
                "Start date and time must be before end date and time.",
                new[] { nameof(StartDateTime), nameof(EndDateTime) }
            ));
        }

        return results;
    }
}
