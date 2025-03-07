using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ParkingAPI.Controllers
{
    [Route("api/parking/zones")]
    [ApiController]
    public class ParkingZonesController : ControllerBase
    {
        private static readonly List<string> Zones = new List<string> { "A", "B", "C" };

        [HttpGet]
        public IActionResult GetParkingZones()
        {
            return Ok(Zones);
        }
    }
}
