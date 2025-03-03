using Microsoft.AspNetCore.Mvc;
using ParkingAPI.Models;
using ParkingAPI.Services;

namespace ParkingAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.GenerateJwtToken(request.Username, request.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { token });
        }
    }
}