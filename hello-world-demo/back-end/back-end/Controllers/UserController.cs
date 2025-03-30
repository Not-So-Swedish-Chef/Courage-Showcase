using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using back_end.Models.Api;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IHostService _hostService; // Inject HostService for host-related logic
        private readonly IUserService _userService; // For saved events endpoints
        private readonly IMapper _mapper;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtService jwtService,
            IConfiguration configuration,
            IMapper mapper,
            IHostService hostService,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
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

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");
            if (user.UserType == UserType.Host)
            {
                await _userManager.AddToRoleAsync(user, "Host");
                await _hostService.CreateHostAsync(user);
            }

            return Ok(new { Message = "User registered successfully" });
        }

        // POST: api/User/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel request)
        {
            var result = await _jwtService.Authenticate(request);
            if (result == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("saved")]
        public async Task<ActionResult<IEnumerable<Event>>> GetSavedEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var savedEvents = await _userService.GetSavedEventsAsync(int.Parse(userId));
            var eventDtos = _mapper.Map<List<EventDTO>>(savedEvents);
            return Ok(eventDtos);
        }

        // POST: api/User/save/{eventId}
        [HttpPost("save/{eventId}")]
        public async Task<IActionResult> SaveEvent(int eventId)
        {
            // Retrieve the currently authenticated user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            var result = await _userService.SaveEventAsync(user.Id, eventId);
            if (!result)
                return BadRequest("Unable to save event.");

            return Ok("Event saved successfully.");
        }


        // DELETE: api/User/save/{eventId}
        [HttpDelete("save/{eventId}")]
        public async Task<IActionResult> RemoveSavedEvent(int eventId)
        {
            // Retrieve the currently authenticated user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            // Remove the event from the user's saved events.
            var result = await _userService.RemoveSavedEventAsync(user.Id, eventId);
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

        public UserType UserType { get; set; } = UserType.Member;
    }    
}
