using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingAPI.Data;
using ParkingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/reservations")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public ReservationsController(DatabaseContext context)
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
}
