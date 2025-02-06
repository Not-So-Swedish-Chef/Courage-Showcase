using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class DemoUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
