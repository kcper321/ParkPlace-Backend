using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ParkingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ParkingAPI.Data
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ParkingContext _context;
        public AuthController(ParkingContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ParkingAPI.Models.User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Email already in use");
            }

            user.Password = HashPassword(user.Password);
            user.RegistrationDate = user.RegistrationDate ?? DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginJson request)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Login);
            if (user == null || user.Password != HashPassword(request.Password))
            {
                return Unauthorized("Invalid credencials");
            }
            var token = GenerateJwtToken(user);
            Console.WriteLine($"User {user.Email} logged in successfully");
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var jwtkey = _config["JwtSettings:secret"];
            if (string.IsNullOrEmpty(jwtkey))
            {
                throw new Exception("Jwt key is missing");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Seed"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JwtSettings:secret");

            var token = new JwtSecurityToken(
                issuer: "https://localhost:5156",
                audience: "https://localhost:5156",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );
            return tokenHandler.WriteToken(token);
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

    }
}

