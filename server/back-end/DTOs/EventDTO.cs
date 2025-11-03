using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace back_end.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "";

        [Required]
        [MaxLength(300)]
        public string Location { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } = 0;

        [Url]
        public string Url { get; set; } = "";

        public int HostId { get; set; }

        [Range(0, 150)]
        public int? MinAge { get; set; }

        [Range(0, 150)]
        public int? MaxAge { get; set; }

        public List<string>? DisabilityTags { get; set; }

        public int Status { get; set; } = 0;
    }
}
