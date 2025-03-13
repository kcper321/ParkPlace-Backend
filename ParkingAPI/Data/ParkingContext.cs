using Microsoft.EntityFrameworkCore;
using ParkingAPI.Models;
using System.Collections.Generic;

namespace ParkingAPI.Data
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options) { }

        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<Reservation>Reservations { get; set; }
        public DbSet<UserDb> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasKey(r => new { r.Id });
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ParkingSpot)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.ParkingSpotId);
        }

        
    }
}