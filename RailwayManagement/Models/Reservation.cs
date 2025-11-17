using System.ComponentModel.DataAnnotations;
namespace RailwayManagement.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string PNR { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int TrainId { get; set; }
        public Train Train { get; set; }
        
        [Required]
        public DateTime JourneyDate { get; set; }
        
        [Required]
        [Range(1, 6)]
        public int NumberOfPassengers { get; set; }
        
        [Required]
        public decimal TotalFare { get; set; }
        
        public ReservationStatus Status { get; set; } = ReservationStatus.PendingPayment;
        
        public DateTime BookingDate { get; set; } = DateTime.Now;
        
        public ICollection<Passenger> Passengers { get; set; }
        public Payment? Payment { get; set; }
    }
}