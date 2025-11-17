using System.ComponentModel.DataAnnotations;
using RailwayManagement.Models;

namespace RailwayManagement.DTOs
{
    public class PaymentRequest
    {
        [Required]
        public int ReservationId { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; }
        
        // Credit/Debit Card fields
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        
        // UPI fields
        public string? UpiId { get; set; }
        
        // Net Banking fields
        public string? BankCode { get; set; }
        public string? AccountNumber { get; set; }
    }
    
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public int RetryAttempt { get; set; } = 0;
    }
    


}