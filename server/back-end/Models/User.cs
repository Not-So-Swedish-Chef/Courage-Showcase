using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using back_end.Enums;

namespace back_end.Models
{
    public class User: IdentityUser<int>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; } = "";
        public UserType UserType { get; set; } = UserType.Member;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public DateTime? SuspensionEndDate { get; set; }
        public ICollection<Event> SavedEvents { get; set; } = new List<Event>();
    }
}
