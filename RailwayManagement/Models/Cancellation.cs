using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class Cancellation
    {
        public int Id { get; set; }
        
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        
        [Required]
        public string Reason { get; set; }
        
        public decimal RefundAmount { get; set; }
        
        public string RefundStatus { get; set; } = "Pending"; // Pending, Processed
        
        public DateTime CancellationDate { get; set; } = DateTime.Now;
    }
}