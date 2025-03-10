using back_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    public class AuthController : Controller
    {

        [HttpPost("signup")]
        public IActionResult signUp([FromBody] SignUpFormData signUpData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processing 
            // No need to handle this for Sprint 1

            return Ok(new { message = "Sign-up successful" });
        }
    }
}
