using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace back_end.Controllers
{
    [Authorize(Roles = "Host")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger, IEventService eventService)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return Ok(await _eventService.GetAllEventsAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null) return NotFound();
            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
        {
            if (eventItem == null) return BadRequest();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userType = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userType))
            {
                return Unauthorized("User claims missing.");
            }

            if (!TryValidateModel(eventItem))
            {
                return BadRequest(ModelState);
            }

            await _eventService.AddEventAsync(eventItem);

            return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, eventItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventItem)
        {
            if (eventItem == null || id == 0) return BadRequest();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != eventItem.HostId.ToString())
            {
                return Unauthorized();
            }
            await _eventService.UpdateEventAsync(eventItem, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // Fetch the event first to check the creator
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != eventItem.HostId.ToString())
            {
                return Unauthorized(); 
            }
            await _eventService.DeleteEventAsync(id, userId);
            return NoContent();
        }

    }
}
