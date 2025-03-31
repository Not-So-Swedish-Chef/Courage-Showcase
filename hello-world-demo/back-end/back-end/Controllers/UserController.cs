using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
using Microsoft.Extensions.Logging;

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
        private readonly IHostService _hostService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtService jwtService,
            IConfiguration configuration,
            IMapper mapper,
            IHostService hostService,
            IUserService userService,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _configuration = configuration;
            _hostService = hostService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
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
                    _logger.LogWarning("User registration failed for {Email}.", model.Email);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel request)
        {
            try
            {
                var result = await _jwtService.Authenticate(request);
                if (result == null)
                {
                    _logger.LogWarning("Invalid login attempt for {Email}.", request.Email);
                    return Unauthorized(new { Message = "Invalid credentials" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login.");
                return StatusCode(500, "An error occurred while logging in.");
            }
        }

        [Authorize]
        [HttpGet("saved")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetSavedEvents()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not found in claims.");
                    return Unauthorized("User not found.");
                }

                var savedEvents = await _userService.GetSavedEventsAsync(int.Parse(userId));
                var eventDtos = _mapper.Map<List<EventDTO>>(savedEvents);
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching saved events.");
                return StatusCode(500, "An error occurred while retrieving saved events.");
            }
        }

        [Authorize]
        [HttpPost("save/{eventId}")]
        public async Task<IActionResult> SaveEvent(int eventId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not found in claims.");
                    return Unauthorized("User not found.");
                }

                var result = await _userService.SaveEventAsync(int.Parse(userId), eventId);
                if (!result)
                {
                    _logger.LogWarning("Unable to save event for user {UserId} with event ID {EventId}.", userId, eventId);
                    return BadRequest("Unable to save event.");
                }

                return Ok("Event saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving the event.");
                return StatusCode(500, "An error occurred while saving the event.");
            }
        }

        [Authorize]
        [HttpDelete("save/{eventId}")]
        public async Task<IActionResult> RemoveSavedEvent(int eventId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not found in claims.");
                    return Unauthorized("User not found.");
                }

                var result = await _userService.RemoveSavedEventAsync(int.Parse(userId), eventId);
                if (!result)
                {
                    _logger.LogWarning("Unable to remove saved event for user {UserId} with event ID {EventId}.", userId, eventId);
                    return BadRequest("Unable to remove event.");
                }

                return Ok("Event removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing the saved event.");
                return StatusCode(500, "An error occurred while removing the event.");
            }
        }
    }

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
