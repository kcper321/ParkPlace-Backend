namespace ParkingAPI.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public int ParkingLotId { get; set; }
        public string Location { get; set; }
        public int AvailableSpots { get; set; }
    }
}
