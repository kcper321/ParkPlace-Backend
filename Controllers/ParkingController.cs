using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ParkingAPI.Data;
using ParkingAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ParkingAPI.Controllers  
{
    [Route("api/auth")]
    [ApiController]

    public class ParkingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public  ParkingController( ApplicationDbContext context){
            _context = context;
        }
                public static List<ParkingSpot> parkingSpots = new List<ParkingSpot>
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
        public async Task<ActionResult<IEnumerable<ParkingSpot>>> GetAvailableParkingSpots()
        {
            var availableSpots = await _context.ParkingSpots
                .Where(p => p.AvailableSpots > 0)
                    .ToListAsync();

            return Ok(availableSpots);
        }
    }

    
    public class ParkingSpot
    {
        public int Id { get; set; }
        public required string Location { get; set; }
        public int AvailableSpots { get; set; }
    }
}
