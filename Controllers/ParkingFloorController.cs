using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ParkingAPI.Controllers
{
    [Route("api/parking/floors")]
    [ApiController]
    public class ParkingFloorsController : ControllerBase
    {
        private static readonly List<int> Floors = new List<int> { 1, 2, 3 };

        [HttpGet]
        public IActionResult GetParkingFloors()
        {
            return Ok(Floors);
        }
    }
}
