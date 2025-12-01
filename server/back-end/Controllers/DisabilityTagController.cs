using back_end.Models;
using back_end.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisabilityTagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DisabilityTagController> _logger;

        public DisabilityTagController(ApplicationDbContext context, ILogger<DisabilityTagController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/DisabilityTag
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTags()
        {
            try
            {
                var tags = await _context.DisabilityTags
                    .Select(t => new 
                    {
                        t.Id,
                        t.Name,
                        t.NormalizedName,
                        EventCount = t.Events.Count(e => e.Status != back_end.Enums.EventStatus.Canceled)
                    })
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving disability tags.");
                return StatusCode(500, new { message = "An error occurred while retrieving disability tags." });
            }
        }

        // GET: api/DisabilityTag/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DisabilityTag>> GetTagById(int id)
        {
            try
            {
                var tag = await _context.DisabilityTags.FindAsync(id);
                if (tag == null)
                {
                    return NotFound();
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving disability tag with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the disability tag." });
            }
        }

        // POST: api/DisabilityTag
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DisabilityTag>> CreateTag([FromBody] CreateDisabilityTagDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { message = "Tag name is required." });
                }

                var normalizedName = TagNormalizer.Normalize(dto.Name);

                // Check if tag already exists
                var existingTag = await _context.DisabilityTags
                    .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName);

                if (existingTag != null)
                {
                    return Conflict(new { message = $"A tag with the name '{dto.Name}' already exists." });
                }

                var tag = new DisabilityTag
                {
                    Name = dto.Name.Trim(),
                    NormalizedName = normalizedName
                };

                _context.DisabilityTags.Add(tag);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating disability tag.");
                return StatusCode(500, new { message = "An error occurred while creating the disability tag." });
            }
        }

        // PUT: api/DisabilityTag/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateDisabilityTagDTO dto)
        {
            try
            {
                var tag = await _context.DisabilityTags.FindAsync(id);
                if (tag == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { message = "Tag name is required." });
                }

                var normalizedName = TagNormalizer.Normalize(dto.Name);

                // Check if another tag with this name exists
                var existingTag = await _context.DisabilityTags
                    .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName && t.Id != id);

                if (existingTag != null)
                {
                    return Conflict(new { message = $"A tag with the name '{dto.Name}' already exists." });
                }

                tag.Name = dto.Name.Trim();
                tag.NormalizedName = normalizedName;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating disability tag with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while updating the disability tag." });
            }
        }

        // DELETE: api/DisabilityTag/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var tag = await _context.DisabilityTags
                    .Include(t => t.Events)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tag == null)
                {
                    return NotFound();
                }

                // Check if tag is being used by any active events (exclude canceled events)
                var activeEvents = tag.Events.Where(e => e.Status != back_end.Enums.EventStatus.Canceled).ToList();
                if (activeEvents.Any())
                {
                    return BadRequest(new { message = $"Cannot delete tag '{tag.Name}' because it is being used by {activeEvents.Count} active event(s)." });
                }

                _context.DisabilityTags.Remove(tag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting disability tag with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the disability tag." });
            }
        }
    }

    public class CreateDisabilityTagDTO
    {
        public string Name { get; set; } = "";
    }

    public class UpdateDisabilityTagDTO
    {
        public string Name { get; set; } = "";
    }
}
