
namespace back_end.Models.Api
{
    public class LoginResponseModel
    {
        public string? Token { get; set; }
        public UserRes? SignedInUser { get; set; }
    }

    public class UserRes
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public UserType UserType { get; set; }
    }
}
