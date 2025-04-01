using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace back_end.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Location { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public decimal Price { get; set; } = 0;

        public string Url { get; set; } = "";
    }
}
