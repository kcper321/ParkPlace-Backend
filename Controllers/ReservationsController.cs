using Microsoft.AspNetCore.Mvc;

[Route("api/reservations")]
[ApiController]
public class ReservationsController : ControllerBase
{
    [HttpGet("history")]
    public IActionResult GetReservationHistory()
    {
        var reservations = new List<object>
        {
            new { Id = 1, UserId = 101, ParkingSpot = "A1", Date = "2025-02-25" },
            new { Id = 2, UserId = 102, ParkingSpot = "B3", Date = "2025-02-24" }
        };

        return Ok(reservations);
    }
}