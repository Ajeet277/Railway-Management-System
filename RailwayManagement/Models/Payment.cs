using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, UPI, NetBanking
        
        public string TransactionId { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed
        
        public string? FailureReason { get; set; }
        
        public int RetryAttempts { get; set; } = 0;
        
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        public DateTime? CompletedAt { get; set; }
        
        // Payment method specific fields
        public string? CardLast4Digits { get; set; }
        public string? UpiId { get; set; }
        public string? BankName { get; set; }
    }
}