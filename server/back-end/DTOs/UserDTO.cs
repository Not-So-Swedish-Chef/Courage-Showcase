using back_end.Enums;
using back_end.Models;

namespace back_end.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserType UserType { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
    }
}
