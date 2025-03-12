using System.ComponentModel.DataAnnotations;

namespace ParkingAPI.Models
{
    public class ParkingSpotJson
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public string? Zone { get; set; }
        public int Number { get; set; }

    }
    public class ReservationJson
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EntireDay { get; set; }
        public int Status { get; set; }

        public ParkingSpotJson? ParkingSpot { get; set; }
    }

    public class UserJson
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(50)]
        public string? Surname { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(256)]
        public string? Password { get; set; }
        public bool AdminUser { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}