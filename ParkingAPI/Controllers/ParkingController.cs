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
                .AnyAsync(r => r.ParkingSpotId == request.ParkingSpotId && r.Status == true && ((r.StartTime <= request.EndTime && r.EndTime >= request.StartTime) || r.Day));
        
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
                Status = true
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Parking spot reserved successfully.", reservationId = reservation.Id });

        }

        [HttpGet("user/{userId}/reservation")]
        public async Task<IActionResult> GetUserReservation(int userId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId && r.Status == true)
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
            if (reservation == null || reservation.Status == false)
            {
                return NotFound("Reservation not found or already canceled");
            }
            reservation.Status = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reservation canceled successfully." });
        }


    [HttpGet("available-spots")]
        public async Task<ActionResult<IEnumerable<ParkingSpot>>> GetAvailableParkingSpots()
        {
            var reservedSpotIds = await _context.Reservations
                .Where(r => r.Status == true)
                .Select(r => r.ParkingSpotId)
                .ToListAsync();

            var availableSpots = await _context.ParkingSpots
                .Where(p => !reservedSpotIds.Contains(p.Id))
                    .ToListAsync();

            return Ok(availableSpots);
        }
    }

}