using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
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
        private readonly IMapper _mapper;


        public EventController(ILogger<EventController> logger, IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                var eventDtos = _mapper.Map<List<EventDTO>>(events);
                return Ok(eventDtos);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving events.");
                return StatusCode(500, "An error occurred while retrieving events.");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            try
            {
                var eventItem = await _eventService.GetEventByIdAsync(id);
                if (eventItem == null) return NotFound();
                return Ok(eventItem);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving event with ID: {id}");
                return StatusCode(500, "An error occurred while retrieving the event.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
        {
            try
            {
                if (eventItem == null) return BadRequest("Event data is missing.");

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
            catch (DataException ex)
            {
                _logger.LogError(ex, "Error occurred while creating event.");
                return StatusCode(500, "An error occurred while creating the event.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventItem)
        {
            try
            {
                if (eventItem == null || id == 0) return BadRequest("Invalid event data.");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != eventItem.HostId.ToString())
                {
                    return Unauthorized("You are not authorized to update this event.");
                }

                await _eventService.UpdateEventAsync(eventItem, userId);
                return NoContent();
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, $"Error occurred while updating event with ID: {id}");
                return StatusCode(500, "An error occurred while updating the event.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var eventItem = await _eventService.GetEventByIdAsync(id);
                if (eventItem == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != eventItem.HostId.ToString())
                {
                    return Unauthorized("You are not authorized to delete this event.");
                }

                await _eventService.DeleteEventAsync(id, userId);
                return NoContent();
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting event with ID: {id}");
                return StatusCode(500, "An error occurred while deleting the event.");
            }
        }
    }
}
