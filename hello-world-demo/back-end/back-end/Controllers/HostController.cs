using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace back_end.Controllers
{
    [Authorize(Roles = "Host")]
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : ControllerBase
    {
        private readonly IHostService _hostService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<HostController> _logger;

        public HostController(IHostService hostService, UserManager<User> userManager, IMapper mapper, ILogger<HostController> logger)
        {
            _hostService = hostService ?? throw new ArgumentNullException(nameof(hostService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetMyEvents()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not found in claims.");
                    return Unauthorized("User not found.");
                }

                var host = await _hostService.GetHostByUserIdAsync(int.Parse(userId));
                if (host == null)
                {
                    _logger.LogWarning($"Host profile not found for user ID {userId}.");
                    return NotFound("Host profile not found.");
                }

                var eventDtos = _mapper.Map<List<EventDTO>>(host.Events);
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting events for the host.");
                return StatusCode(500, "An error occurred while retrieving the events.");
            }
        }

        
        [HttpPut]
        public async Task<IActionResult> UpdateHostInfo([FromBody] UpdateHostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state when updating host info.");
                    return BadRequest(ModelState);
                }

                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                bool result = await _hostService.UpdateHostInfoAsync(userId, model);
                if (!result)
                {
                    _logger.LogWarning($"Unable to update host info for user ID {userId}.");
                    return BadRequest("Unable to update host info.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating host profile.");
                return StatusCode(500, "An error occurred while updating the host information.");
            }
        }
    }

    public class UpdateHostModel
    {
        public string? AgencyName { get; set; }
        public string? Bio { get; set; }
    }
}
