using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParkingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminResource()
        {
            return Ok(new { message = "This is restricted to Admin users." });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("user-or-admin")]
        public IActionResult GetUserOrAdminResource()
        {
            return Ok(new { message = "This is available to both Users and Admins." });
        }
    }
}