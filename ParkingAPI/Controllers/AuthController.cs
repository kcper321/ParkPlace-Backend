using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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
            
            var UserDb = new UserDb()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user?.Email,
                Password = user.PasswordHash,
                AdminUser = user.AdminUser,
                RegistrationDate = DateTime.Now
            };
            _context.Users.Add(UserDb);
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

            
            private string GenerateJwtToken(UserDb user){
            if(user == null || string.IsNullOrEmpty(user.Email)){
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var jwtkey = _config["JwtSettings:secret"];
            if(string.IsNullOrEmpty(jwtkey)){
                throw new Exception ("Jwt key is missing");
            }
             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("33df9570a04b4298dc2da053a50b4966a0c0e1aaea73b86bf58b63862af09c6df5e16afc4487658b2794d6b28c7c27adb49c4298b803b9e39c54f3598d6c31c7af13ac06d3e750b98cd00ba3c31500a6fd1b022994b68df4bb461d5227382353dd141ae41039d0f26ab372b167f26348b9b8a35c21fd2228503b9854d3ccc1f53de54531c3bf725bee887af65f4c91f22bed7347cd81634655cc701a823ecc970864f8035b40d17fbd794095068ea7274297b6426783af445badfaf262e6e3c7bcbeeb7dff0bfa2ceaaf7315c4f8ca9ab49e024be13ba71cf3548298412041ea59d656624c452306625df083ab7f5577396dfea003c681a5e800918f2228718c"));
             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
             var claims = new []
             {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JwtSettings:secret");
            
            var token = new JwtSecurityToken(
                issuer : "https://localhost:5156",
                audience : "https://localhost:5156",
                claims : claims,
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
 

           

        //     [HttpGet("user")]
        //     [Authorize]
        //     public IActionResult GetUserData()
        //     {
        //         var userId = User.FindFirst("id")?.Value;
        //         var UserName = User.Identity?.Name;
        //         return Ok(new { UserId = userId, UserName = UserName });
        //     }
        //      private string GenerateJwtToken(UserJson user){
        //     if(user == null || string.IsNullOrEmpty(user.Email)){
        //         throw new ArgumentNullException(nameof(user), "User cannot be null.");
        //     }
        //     var jwtkey = _config["Jwt:Key"];
        //     if(string.IsNullOrEmpty(jwtkey)){
        //         throw new Exception ("Jwt key is missing");
        //     }
        //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("11111111111111111111111111111111"));
        //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //     var claims = new []
        //     {
        //         new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //     };
           
        //     var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["Jwt:ExpireDays"]));
           
        //     var token = new JwtSecurityToken(
        //         issuer : "https://localhost:5112",
        //         audience : "https://localhost:5112",
        //         claims : claims,
        //         expires: DateTime.UtcNow.AddHours(2),
        //         signingCredentials: credentials
        //     );
        //     return new JwtSecurityTokenHandler().WriteToken(token);
            
        // }
   
    