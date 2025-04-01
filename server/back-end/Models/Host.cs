using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.Models;
public class Host
{
    [Key]
    [ForeignKey("User")] 
    public int Id { get; set; }
    public string? AgencyName { get; set; }
    public string? Bio { get; set; }
    public virtual User User { get; set; } = new User();
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
