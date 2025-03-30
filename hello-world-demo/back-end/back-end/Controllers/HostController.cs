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
using static NuGet.Packaging.PackagingConstants;

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

        // Constructor with both IHostService and UserManager<User> injected
        public HostController(IHostService hostService, UserManager<User> userManager, IMapper mapper)
        {
            _hostService = hostService ?? throw new ArgumentNullException(nameof(hostService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper;
        }

        // GET: api/Host/events
        // Returns all events created by the authenticated host.
        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<Event>>> GetMyEvents()
        {
            // Retrieve the currently authenticated user from HttpContext.
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            // Get the host profile by user ID.
            var host = await _hostService.GetHostByUserIdAsync(int.Parse(userId));
            if (host == null)
                return NotFound("Host profile not found.");

            var eventDtos = _mapper.Map<List<EventDTO>>(host.Events);
            return Ok(eventDtos);
        }

        // PUT: api/Host
        // Allows a host to update their profile information.
        [HttpPut]
        public async Task<IActionResult> UpdateHostInfo([FromBody] UpdateHostModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool result = await _hostService.UpdateHostInfoAsync(userId, model);
            if (!result)
                return BadRequest("Unable to update host info.");

            return NoContent();
        }
    }

    // Model to update host information.
    public class UpdateHostModel
    {
        public string? AgencyName { get; set; }
        public string? Bio { get; set; }
    }
}
