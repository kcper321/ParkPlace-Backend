using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ParkingAPI.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
                private static List<ParkingSpot> parkingSpots = new List<ParkingSpot>
        {
            new ParkingSpot { Id = 1, Location = "Floor 1, Zone A", AvailableSpots = 5 },
            new ParkingSpot { Id = 2, Location = "Floor 2, Zone B", AvailableSpots = 3 },
            new ParkingSpot { Id = 3, Location = "Floor 1, Zone C", AvailableSpots = 0 }
        };

        
        [HttpGet]
        public ActionResult<IEnumerable<ParkingSpot>> GetParkingSpots()
        {
            return Ok(parkingSpots);
        }

       
        [HttpGet("available")]
        public ActionResult<IEnumerable<ParkingSpot>> GetAvailableSpots()
        {
            var availableSpots = parkingSpots.Where(p => p.AvailableSpots > 0).ToList();
            return Ok(availableSpots);
        }
    }

    
    public class ParkingSpot
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public int AvailableSpots { get; set; }
    }
}
