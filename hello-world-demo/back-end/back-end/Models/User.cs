using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Used as the primary identifier for each user
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string AccountType { get; set; }
    }
}
