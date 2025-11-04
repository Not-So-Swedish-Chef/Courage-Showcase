using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserService userService,
            IMapper mapper,
            ILogger<AdminController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userDtos = _mapper.Map<List<UserDTO>>(users);
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all users.");
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }

        [HttpPost("suspend")]
        public async Task<IActionResult> SuspendUser([FromBody] SuspendUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.SuspendUserAsync(request.UserId, request.Days);
                if (!result)
                {
                    _logger.LogWarning("Unable to suspend user with ID {UserId}.", request.UserId);
                    return BadRequest("Unable to suspend user. User may not exist.");
                }

                return Ok($"User {request.UserId} has been suspended for {request.Days} days.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while suspending user {UserId}.", request.UserId);
                return StatusCode(500, "An error occurred while suspending the user.");
            }
        }

        [HttpPost("ban")]
        public async Task<IActionResult> BanUser([FromBody] BanUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.BanUserAsync(request.UserId);
                if (!result)
                {
                    _logger.LogWarning("Unable to ban user with ID {UserId}.", request.UserId);
                    return BadRequest("Unable to ban user. User may not exist.");
                }

                return Ok($"User {request.UserId} has been permanently banned.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while banning user {UserId}.", request.UserId);
                return StatusCode(500, "An error occurred while banning the user.");
            }
        }

        [HttpPost("unsuspend")]
        public async Task<IActionResult> UnsuspendUser([FromBody] UnsuspendUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.UnsuspendUserAsync(request.UserId);
                if (!result)
                {
                    _logger.LogWarning("Unable to unsuspend user with ID {UserId}.", request.UserId);
                    return BadRequest("Unable to unsuspend user. User may not exist or is not suspended.");
                }

                return Ok($"User {request.UserId} has been unsuspended.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unsuspending user {UserId}.", request.UserId);
                return StatusCode(500, "An error occurred while unsuspending the user.");
            }
        }

        [HttpPost("unban")]
        public async Task<IActionResult> UnbanUser([FromBody] UnbanUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.UnbanUserAsync(request.UserId);
                if (!result)
                {
                    _logger.LogWarning("Unable to unban user with ID {UserId}.", request.UserId);
                    return BadRequest("Unable to unban user. User may not exist or is not banned.");
                }

                return Ok($"User {request.UserId} has been unbanned.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unbanning user {UserId}.", request.UserId);
                return StatusCode(500, "An error occurred while unbanning the user.");
            }
        }
    }

    public class SuspendUserRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "Days must be between 1 and 365.")]
        public int Days { get; set; }
    }

    public class BanUserRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class UnsuspendUserRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class UnbanUserRequest
    {
        [Required]
        public int UserId { get; set; }
    }
}
