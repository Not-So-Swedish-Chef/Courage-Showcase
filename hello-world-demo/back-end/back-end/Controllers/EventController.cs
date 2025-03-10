using back_end.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult createEvent([FromBody] EventFormData eventData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processing 

            return Ok(new { message = "Event created successfully" });
        }
    }
}
