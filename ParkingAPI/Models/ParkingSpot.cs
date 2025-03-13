

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
        public int EntireDay { get; set; }
        public int Status { get; set; }

        public ParkingSpot? ParkingSpot { get; set; }
    }
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AdminUser { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }

}