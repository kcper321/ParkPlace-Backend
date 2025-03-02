using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ParkingAPI.Models;

namespace ParkingAPI.Data;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options){}
        
            public override DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        
        
        }
