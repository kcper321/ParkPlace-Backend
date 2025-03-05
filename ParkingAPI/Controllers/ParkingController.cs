using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using ParkingAPI.Models;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly ParkingContext _context;

        public ParkingController(ParkingContext context)
        {
            _context = context;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveParkingSpot([FromBody] Reservation request)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(request.ParkingSpotId);
            if (parkingSpot == null)
            {
                return NotFound("Parking spot not found.");
            }

            bool isReserved = await _context.Reservations
                .AnyAsync(r => r.ParkingSpotId == request.ParkingSpotId && r.Status == 1 && ((r.StartTime <= request.EndTime && r.EndTime >= request.StartTime) || r.Day == 1));
        
            if (isReserved)
            {
                return Conflict("Parking spot is already reserved for this time.");
            }
            var reservation = new Reservation
            {
                UserId = request.UserId,
                ParkingSpotId = request.ParkingSpotId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Day = request.Day,
                Status = 1
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Parking spot reserved successfully.", reservationId = reservation.Id });

        }

        [HttpGet("user/{userId}/reservation")]
        public async Task<IActionResult> GetUserReservation(int userId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId && r.Status == 1)
                .Include(r => r.ParkingSpot)
                .ToListAsync();
            if (reservations.Count == 0)
            {
                return NotFound("No reservations foud for this user.");
            }
            return Ok(reservations);
        }

        [HttpDelete("cancel/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null || reservation.Status == 0)
            {
                return NotFound("Reservation not found or already canceled");
            }
            reservation.Status = 0;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reservation canceled successfully." });
        }


    [HttpGet("available-spots")]
        public async Task<ActionResult<IEnumerable<ParkingSpot>>> GetAvailableParkingSpots()
        {
            //var availableSpots = from ParkingSpot in _context.ParkingSpots
            //                     join Reservation in _context.Reservations on ParkingSpot.Id equals Reservation.ParkingSpotId
            //                     where Reservation.Status == 0
            //                     select new ParkingSpot()
            //                     {
            //                         Id = ParkingSpot.Id
            //                     };
            var reservedSpotIds = await _context.Reservations
                .Where(r => r.Status == 1)
                .Select(r => r.ParkingSpotId)
                .ToListAsync();

            var availableSpots = await _context.ParkingSpots
                .Where(p => !reservedSpotIds.Contains(p.Id))
                    .ToListAsync();

            return Ok(availableSpots);
            //return Ok(availableSpots.ToList());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Email already in use");
            }
            user.Password = HashPassword(user.Password);
            user.RegistrationDate = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Password != HashPassword(password))
            {
                return Unauthorized("Invalid credencials");
            }
            return Ok(new { Message = "Login Successful", user.Name, user.Surname, user.AdminUser });
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