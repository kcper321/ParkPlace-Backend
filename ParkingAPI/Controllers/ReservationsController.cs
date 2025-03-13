using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingAPI.Data;
using ParkingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ParkingAPI.Controllers
{
    [Route("api/parking/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ParkingContext _context;

        public ReservationsController(ParkingContext context)
        {
            _context = context;
        }


        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetReservationHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.ParkingSpotId,
                    r.StartTime,
                    r.EndTime,
                    r.EntireDay,
                    r.Status
                })
                .ToListAsync();

            return Ok(reservations);
        }


        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveParkingSpot([FromBody] ReservationJson request)
        {
            var reservation = new Reservation()
            {
                EntireDay = request.EntireDay,
                UserId = request.UserId,
                ParkingSpotId = request.ParkingSpotId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = 1
            };

            var parkingSpot = await _context.ParkingSpots.FindAsync(request.ParkingSpotId);
            if (parkingSpot == null)
            {
                return NotFound("Parking spot not found.");
            }

            bool isReserved = await _context.Reservations
                .AnyAsync(r => r.ParkingSpotId == request.ParkingSpotId && r.Status == 1 &&
                               ((r.StartTime <= request.EndTime && r.EndTime >= request.StartTime) || r.EntireDay == 1));

            if (isReserved)
            {
                return Conflict("Parking spot is already reserved for this time.");
            }

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
                return NotFound("No reservations found for this user.");
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
            var reservedSpotIds = await _context.Reservations
                .Where(r => r.Status == 1)
                .Select(r => r.ParkingSpotId)
                .ToListAsync();

            var availableSpots = await _context.ParkingSpots
                .Where(p => !reservedSpotIds.Contains(p.Id))
                .ToListAsync();

            return Ok(availableSpots);
        }
    }
}
