using Microsoft.EntityFrameworkCore;
using RailwayManagement.Models;

namespace RailwayManagement.Data
{
    public class RailwayDbContext : DbContext
    {
        public RailwayDbContext(DbContextOptions<RailwayDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<Admin> Admins { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();
                
            modelBuilder.Entity<Train>()
                .HasIndex(t => t.TrainNumber)
                .IsUnique();
                
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.PNR)
                .IsUnique();
                
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Reservation)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.ReservationId);
        }
    }
}