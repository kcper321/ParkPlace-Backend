using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using ParkingAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParkingController : ControllerBase
    {
        private readonly ParkingContext _context;

        public ParkingController(ParkingContext context)
        {
            _context = context;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveParkingSpot([FromBody] ReservationJson request)
        {

            Reservation reservation = new Reservation()
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
                .AnyAsync(r => r.ParkingSpotId == request.ParkingSpotId && r.Status == 1 && ((r.StartTime <= request.EndTime && r.EndTime >= request.StartTime) || r.EntireDay == 1));
        
            if (isReserved)
            {
                return Conflict("Parking spot is already reserved for this time.");
            }
            //var reservation = new Reservation
            //{
            //    UserId = request.UserId,
            //    ParkingSpotId = request.ParkingSpotId,
            //    StartTime = request.StartTime,
            //    EndTime = request.EndTime,
            //    Day = request.Day,
            //    Status = 1
            //};

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

        [HttpGet("users/me")]
        public async Task<IActionResult> GetCurrentUser([FromQuery] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { user.Name, user.Surname, user.Email, user.AdminUser });
        }

        [HttpGet("reservations/active")]
        public async Task<IActionResult> GetActiveReservations()
        {
            var activeReservations = await _context.Reservations
                .Where(r => r.Status == 1)
                .Include(r => r.ParkingSpot)
                .ToListAsync();

            if (!activeReservations.Any())
            {
                return NotFound("No active reservations found.");
            }

            var activeReservationsWithUser = activeReservations.Select(r => new
            {
                r.Id,
                r.StartTime,
                r.EndTime,
                r.ParkingSpot,
                r.UserId,
                User = _context.Users.FirstOrDefault(u => u.Id == r.UserId)

            }).ToList();
            return Ok(activeReservationsWithUser);
        }

        
    }

}