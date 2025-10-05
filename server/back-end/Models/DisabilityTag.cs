using System.ComponentModel.DataAnnotations;

public class DisabilityTag
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = ""; 

    [MaxLength(64)]
    public string NormalizedName { get; set; } = ""; 

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
