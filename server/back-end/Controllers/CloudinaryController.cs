using back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Authorize(Roles = "Host, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CloudinaryController> _logger;

        public CloudinaryController(ICloudinaryService cloudinaryService, ILogger<CloudinaryController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [HttpPost("generate-signature")]
        public ActionResult<CloudinaryUploadParams> GenerateUploadSignature([FromQuery] string folder = "events")
        {
            try
            {
                var uploadParams = _cloudinaryService.GenerateUploadSignature(folder);
                return Ok(uploadParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Cloudinary upload signature");
                return StatusCode(500, new { error = "Failed to generate upload signature" });
            }
        }
    }
}
