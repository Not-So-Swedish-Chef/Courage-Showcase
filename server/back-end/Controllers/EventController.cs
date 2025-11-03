using AutoMapper;
using back_end.DTOs;
using back_end.Enums;
using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Authorize(Roles = "Host, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;


        public EventController(ILogger<EventController> logger, IEventService eventService, IMapper mapper, ApplicationDbContext context)
        {
            _eventService = eventService;
            _logger = logger;
            _mapper = mapper;
            _context = context;
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
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            try
            {
                var eventItem = await _eventService.GetEventByIdAsync(id);
                if (eventItem == null) return NotFound();
                var eventDto = _mapper.Map<EventDTO>(eventItem);
                return Ok(eventDto);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving event with ID: {id}");
                return StatusCode(500, "An error occurred while retrieving the event.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO eventDto)
        {
            try
            {
                if (eventDto == null) return BadRequest("Event data is missing.");

                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var userType = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userType))
                {
                    return Unauthorized("User claims missing.");
                }

                // Parse city string to OntarioCity enum
                if (!Enum.TryParse<OntarioCity>(eventDto.City, true, out var cityEnum))
                {
                    return BadRequest($"Invalid city. Must be a valid Ontario city name (e.g., Toronto, Ottawa, Hamilton).");
                }

                // Map DTO to Event entity
                var eventItem = new Event
                {
                    Title = eventDto.Title,
                    Location = eventDto.Location,
                    City = cityEnum,
                    ImageUrl = eventDto.ImageUrl,
                    StartDateTime = eventDto.StartDateTime,
                    EndDateTime = eventDto.EndDateTime,
                    Price = eventDto.Price,
                    Url = eventDto.Url,
                    HostId = eventDto.HostId,
                    MinAge = eventDto.MinAge,
                    MaxAge = eventDto.MaxAge,
                    Status = (EventStatus)eventDto.Status,
                    DisabilityTags = new List<DisabilityTag>()
                };

                // Handle disability tags - create new ones if they don't exist
                if (eventDto.DisabilityTags != null && eventDto.DisabilityTags.Any())
                {
                    foreach (var tagName in eventDto.DisabilityTags)
                    {
                        if (string.IsNullOrWhiteSpace(tagName)) continue;

                        var normalizedName = back_end.Utils.TagNormalizer.Normalize(tagName);
                        
                        // Check if tag exists
                        var existingTag = await _context.DisabilityTags
                            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName);

                        if (existingTag != null)
                        {
                            eventItem.DisabilityTags.Add(existingTag);
                        }
                        else
                        {
                            // Create new tag
                            var newTag = new DisabilityTag
                            {
                                Name = tagName.Trim(),
                                NormalizedName = normalizedName
                            };
                            _context.DisabilityTags.Add(newTag);
                            eventItem.DisabilityTags.Add(newTag);
                        }
                    }
                }

                if (!TryValidateModel(eventItem))
                {
                    return BadRequest(ModelState);
                }

                await _eventService.AddEventAsync(eventItem);

                // Map back to DTO for response
                var responseDto = _mapper.Map<EventDTO>(eventItem);

                return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, responseDto);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Error occurred while creating event.");
                return StatusCode(500, "An error occurred while creating the event.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDTO eventDto)
        {
            try
            {
                if (eventDto == null || id == 0) return BadRequest("Invalid event data.");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not found.");
                }

                if (userId != eventDto.HostId.ToString())
                {
                    return Unauthorized("You are not authorized to update this event.");
                }

                // Parse city string to OntarioCity enum
                if (!Enum.TryParse<OntarioCity>(eventDto.City, true, out var cityEnum))
                {
                    return BadRequest($"Invalid city. Must be a valid Ontario city name.");
                }

                // Map DTO to Event entity
                var eventItem = new Event
                {
                    Id = id,
                    Title = eventDto.Title,
                    Location = eventDto.Location,
                    City = cityEnum,
                    ImageUrl = eventDto.ImageUrl,
                    StartDateTime = eventDto.StartDateTime,
                    EndDateTime = eventDto.EndDateTime,
                    Price = eventDto.Price,
                    Url = eventDto.Url,
                    HostId = eventDto.HostId,
                    MinAge = eventDto.MinAge,
                    MaxAge = eventDto.MaxAge,
                    Status = (EventStatus)eventDto.Status,
                    DisabilityTags = new List<DisabilityTag>()
                };

                // Handle disability tags
                if (eventDto.DisabilityTags != null && eventDto.DisabilityTags.Any())
                {
                    foreach (var tagName in eventDto.DisabilityTags)
                    {
                        if (string.IsNullOrWhiteSpace(tagName)) continue;

                        var normalizedName = back_end.Utils.TagNormalizer.Normalize(tagName);
                        
                        var existingTag = await _context.DisabilityTags
                            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName);

                        if (existingTag != null)
                        {
                            eventItem.DisabilityTags.Add(existingTag);
                        }
                        else
                        {
                            var newTag = new DisabilityTag
                            {
                                Name = tagName.Trim(),
                                NormalizedName = normalizedName
                            };
                            _context.DisabilityTags.Add(newTag);
                            eventItem.DisabilityTags.Add(newTag);
                        }
                    }
                }

                if (!TryValidateModel(eventItem))
                {
                    return BadRequest(ModelState);
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

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDTO>>> SearchEvents(
            [FromQuery] string? query = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] List<string>? disabilityTags = null,
            [FromQuery] List<string>? cities = null,
            [FromQuery] int? age = null)
        {
            try
            {
                // Apply default values based on requirements
                decimal? effectiveMinPrice = minPrice;
                decimal? effectiveMaxPrice = maxPrice;
                DateTime? effectiveFrom = from;
                DateTime? effectiveTo = to;

                // Price defaults
                if (minPrice.HasValue && !maxPrice.HasValue)
                {
                    effectiveMaxPrice = 200m; // Default max price to $200
                }
                else if (!minPrice.HasValue && maxPrice.HasValue)
                {
                    effectiveMinPrice = 0m; // Default min price to $0
                }

                // Date defaults
                if (from.HasValue && !to.HasValue)
                {
                    // Search from the provided date onwards (no end date limit)
                    effectiveTo = null;
                }
                else if (!from.HasValue && to.HasValue)
                {
                    // Search from current date to the provided date
                    effectiveFrom = DateTime.UtcNow.Date;
                }

                var events = await _eventService.SearchEventsAsync(query, effectiveFrom, effectiveTo, effectiveMinPrice, effectiveMaxPrice, disabilityTags, cities, age);
                var eventDtos = _mapper.Map<List<EventDTO>>(events);
                return Ok(eventDtos);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Error occurred while searching events.");
                return StatusCode(500, "An error occurred while searching events.");
            }
        }
    }
}
