using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHostService _hostService; // Inject HostService for host-related logic
        private readonly IUserService _userService; // For saved events endpoints

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IHostService hostService,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _hostService = hostService;
            _userService = userService;
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create a new user instance.
            var user = new User
            {
                UserName = model.Email, // or another unique username
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = model.UserType // Set the user type (Member or Host)
            };

            // Identity automatically hashes the password.
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // If the user is a Host, create the Host row.
            if (user.UserType == UserType.Host)
            {
                await _hostService.CreateHostAsync(user);
            }

            return Ok(new { Message = "User registered successfully" });
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find the user by email.
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Check the password.
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Generate a JWT token.
            var token = GenerateJwtToken(user);

            // Return token along with user details
            return Ok(new
            {
                Token = token,
                User = new
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = user.UserType.ToString()
                }
            });
        }


        private string GenerateJwtToken(User user)
        {
            // Create claims to include in the token.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Get key and credentials.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
                signingCredentials: creds);

            // Return the serialized JWT.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/User/saved
        [HttpGet("saved")]
        public async Task<ActionResult<IEnumerable<Event>>> GetSavedEvents()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var savedEvents = await _userService.GetSavedEventsAsync(userId);
            return Ok(savedEvents);
        }

        // POST: api/User/save/{eventId}
        [HttpPost("save/{eventId}")]
        public async Task<IActionResult> SaveEvent(int eventId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.SaveEventAsync(userId, eventId);
            if (!result)
                return BadRequest("Unable to save event.");
            return Ok("Event saved successfully.");
        }

        // DELETE: api/User/save/{eventId}
        [HttpDelete("save/{eventId}")]
        public async Task<IActionResult> RemoveSavedEvent(int eventId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.RemoveSavedEventAsync(userId, eventId);
            if (!result)
                return BadRequest("Unable to remove event.");
            return Ok("Event removed successfully.");
        }
    }

    // Extend the registration model to include the user type.
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password should be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // Specify whether the user is a Member or Host.
        public UserType UserType { get; set; } = UserType.Member;
    }

    // Model for user login.
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
