using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Optionally, you could restrict further to Hosts using a Role-based check
    public class HostController : ControllerBase
    {
        private readonly IHostService _hostService;

        public HostController(IHostService hostService)
        {
            _hostService = hostService;
        }

        // GET: api/Host/events
        // Returns all events created by the authenticated host.
        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<Event>>> GetMyEvents()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var host = await _hostService.GetHostByUserIdAsync(userId);
            if (host == null)
                return NotFound("Host profile not found.");

            return Ok(host.Events);
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
