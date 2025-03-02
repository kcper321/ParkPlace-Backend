namespace ParkingAPI.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public int ParkingLotId { get; set; }
        public int Floor { get; set; }
        public char Zone { get; set; }
        public int AvailableSpots { get; set; }
    }
}
