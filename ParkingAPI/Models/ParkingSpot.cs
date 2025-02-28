namespace ParkingAPI.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public string? Zone { get; set; }
        public int Number { get; set; }

        public List<Reservation>? Reservations { get; set; }
    }
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Day { get; set; }
        public bool Status { get; set; }

        public ParkingSpot? ParkingSpot { get; set; }
    }
}