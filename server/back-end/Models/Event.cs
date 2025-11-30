using back_end.Enums;
using back_end.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Event : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = "";

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = "";

    [Required]
    public OntarioCity City { get; set; } = OntarioCity.Toronto;

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

    [Required]
    public int HostId { get; set; }

    [ForeignKey(nameof(HostId))]
    public virtual back_end.Models.Host? Host { get; set; }

    public ICollection<User> UsersWhoSaved { get; set; } = new List<User>();

    [Range(0, 150)]
    public int? MinAge { get; set; }

    [Range(0, 150)]
    public int? MaxAge { get; set; }

    public ICollection<DisabilityTag> DisabilityTags { get; set; } = new List<DisabilityTag>();

    [Required]
    public EventStatus Status { get; set; } = EventStatus.Active;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public Event()
    {
        StartDateTime = DateTime.UtcNow;
        EndDateTime = DateTime.UtcNow.AddHours(1);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (StartDateTime >= EndDateTime)
        {
            results.Add(new ValidationResult(
                "Start date/time must be before end date/time.",
                new[] { nameof(StartDateTime), nameof(EndDateTime) }
            ));
        }

        if (MinAge.HasValue && MaxAge.HasValue && MinAge.Value > MaxAge.Value)
        {
            results.Add(new ValidationResult(
                "MinAge cannot be greater than MaxAge.",
                new[] { nameof(MinAge), nameof(MaxAge) }
            ));
        }

        return results;
    }
}
